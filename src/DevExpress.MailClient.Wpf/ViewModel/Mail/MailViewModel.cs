using System;
using System.Linq;
using System.Collections.ObjectModel;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Windows.Controls;
using DevExpress.Mvvm.POCO;
using System.ComponentModel;
using DevExpress.Mvvm;
using System.Windows;
using DevExpress.MailClient.Helpers;
using System.Collections.Generic;
using DevExpress.Mvvm.DataAnnotations;

namespace DevExpress.MailClient.ViewModel {
    public enum LayoutMode {
        Normal,
        Flipped
    }
    public class MailViewModel : ContentViewModelBase<MailMessageViewModel> {
        public static MailViewModel Create() {
            return ViewModelSource.Create(() => new MailViewModel());
        }

        public virtual bool ShowMessagePreview { get; set; }
        public virtual MailMessageViewModel CurrentMessage { get; set; }
        public virtual MailMessageViewModel NewMessage { get; set; }
        public virtual Orientation LayoutOrientation { get; set; }
        public virtual LayoutMode LayoutMode { get; set; }
        bool isCopyOfCurrentMessage = false;
        IMailFolderDescription currentFolder;
        IEnumerable<IMailFolderDescription> mailFolders;

        protected MailViewModel() : base(ModuleType.Mail) { }

        #region Commands
        public void CreateNewMessage() {
            NewMessage = CreateReplyMessage();
            ShowMessageWindow();
        }
        public void Reply() {
            NewMessage = CreateReplyMessage(CurrentMessage);
            ShowMessageWindow();
        }
        public bool CanReply() {
            return IsMessageActive();
        }
        public void Forward() {
            NewMessage = CreateReplyMessage(CurrentMessage, true);
            ShowMessageWindow();
        }
        public bool CanForward() {
            return IsMessageActive();
        }
        public void Delete() {
            if(CurrentMessage.Type == MessageType.Deleted)
                Items.Remove(CurrentMessage);
            else
                CurrentMessage.Type = MessageType.Deleted;

            if(CurrentMessage.IsUnread)
                UpdateMailUnreadCount(mailFolders);
            ItemsSource.Remove(CurrentMessage);
        }
        public bool CanDelete() {
            return IsMessageActive();
        }
        public void ChangeUnreadStatus() {
            CurrentMessage.IsUnread = !CurrentMessage.IsUnread;
            UpdateMailUnreadCount(mailFolders);
        }
        public bool CanChangeUnreadStatus() {
            return IsMessageActive();
        }
        public void SetLowPriority() {
            CurrentMessage.Priority = MessagePriority.Low;
        }
        public bool CanSetLowPriority() {
            return IsMessageActive();
        }
        public void SetMediumPriority() {
            CurrentMessage.Priority = MessagePriority.Medium;
        }
        public bool CanSetMediumPriority() {
            return IsMessageActive();
        }
        public void SetHighPriority() {
            CurrentMessage.Priority = MessagePriority.High;
        }
        public bool CanSetHighPriority() {
            return IsMessageActive();
        }
        public void ChangeLayout() {
            LayoutOrientation = LayoutOrientation == Orientation.Horizontal ? Orientation.Vertical : Orientation.Horizontal;
        }
        public void FlipLayout() {
            LayoutMode = LayoutMode == LayoutMode.Normal ? LayoutMode.Flipped : LayoutMode.Normal;
        }
        public void SendMessage() {
            EditMessageCore(true, true);
        }
        public void SaveMessage(object parameter) {
            EditMessageCore(false, parameter == null);
        }
        public bool CanSaveMessage(object parameter) {
            return CanSaveMessageCore();
        }
        public void SaveMessageOnClosingConfirmation(object parameter) {
            CancelEventArgs args = parameter as CancelEventArgs;
            MessageBoxResult result = this.GetService<IMessageBoxService>().Show("Do you want to save changes?", "DevExpress Mail Client", MessageBoxButton.YesNoCancel);
            if(result == MessageBoxResult.Cancel) {
                if(args != null)
                    args.Cancel = true;
            } else if(result == MessageBoxResult.OK || result == MessageBoxResult.Yes)
                SaveMessage(parameter);
        }
        public bool CanSaveMessageOnClosingConfirmation(object parameter) {
            return CanSaveMessageCore();
        }
        public void CloseMessageWindow() {
            NewMessage = null;
            this.GetService<IWindowService>().Close();
        }
        public void OpenMessage() {
            NewMessage = MailMessageViewModel.Create();
            NewMessage.Assign(CurrentMessage);
            isCopyOfCurrentMessage = true;
            ShowMessageWindow();
        }
        public bool CanOpenMessage() {
            return currentFolder != null && currentFolder.Type == MessageType.Draft;
        }
        public void MailCustomColumnDisplayText(DevExpress.Xpf.Grid.CustomColumnDisplayTextEventArgs e) {
            if(e.Column.FieldName == "Subject") {
                MailMessageViewModel row = e.Row as MailMessageViewModel;
                e.DisplayText = row != null ? row.SubjectDisplayText : e.Value.ToString();
            }
        }
        bool IsMessageActive() {
            return CurrentMessage != null;
        }
        bool CanSaveMessageCore() {
            return NewMessage != null;
        }
        void EditMessageCore(bool sendMessage, bool closeWindow) {
            if(isCopyOfCurrentMessage) {
                CurrentMessage.Assign(NewMessage);
                NewMessage = CurrentMessage;
            }
            NewMessage.Type = sendMessage ? MessageType.Sent : MessageType.Draft;
            NewMessage.NormalizeEmails();
            if(!Items.Contains(NewMessage)) {
                Items.Add(NewMessage);
                if(!sendMessage) {
                    NewMessage.IsUnread = true;
                    UpdateMailUnreadCount(mailFolders);
                }
            }

            UpdateItemsSource();
            NewMessage = null;
            if(closeWindow)
                this.GetService<IWindowService>().Close();
        }
        #endregion

        [Command(false)]
        public void SetCurrentFolder(IMailFolderDescription folder) {
            currentFolder = folder;
            UpdateItemsSource();
        }
        [Command(false)]
        public void AssignMailFolders(IEnumerable<IMailFolderDescription> folders) {
            mailFolders = folders;
            UpdateMailUnreadCount(mailFolders);
        }
        protected override void InitializeInDesignMode() {
            SetCurrentFolder(MailFolderViewModel.Create(null, null, MessageFolderName.All, MessageType.Inbox));
            CurrentMessage = Items[0];
            NewMessage = CurrentMessage;
            ShowMessagePreview = true;
        }
        protected void OnCurrentMessageChanged() {
            if(CurrentMessage != null && CurrentMessage.IsUnread) {
                CurrentMessage.IsUnread = false;
                UpdateMailUnreadCount(mailFolders);
            }
        }
        protected void OnNewMessageChanged() {
            isCopyOfCurrentMessage = false;
        }
        protected override void UpdateItemsSource() {
            if(currentFolder == null)
                return;

            bool isRootFolder = currentFolder.Folder == MessageFolderName.All || currentFolder.Folder == MessageFolderName.Root;
            var newItems = Items.Where(x => x.Type == currentFolder.Type && (isRootFolder || x.Folder == currentFolder.Folder));
            ItemsSource = new ObservableCollection<MailMessageViewModel>(newItems);
        }
        void UpdateMailUnreadCount(IEnumerable<IMailFolderDescription> folders) {
            if(folders == null)
                return;

            foreach(IMailFolderDescription folder in folders) {
                folder.UnreadCount = GetMessagesUnreadCount(folder);
                UpdateMailUnreadCount(folder.GetSubFolders());
            }
        }
        int GetMessagesUnreadCount(IMailFolderDescription folder) {
            return Items.Count(x => x.IsUnread && (folder.Folder == MessageFolderName.All || x.Folder == folder.Folder) && (x.Type == folder.Type));
        }
        void ShowMessageWindow() {
            this.GetService<IWindowService>().Show(this);
        }

        #region Message Formatting
        MailMessageViewModel CreateReplyMessage(MailMessageViewModel originalMessage = null, bool isForward = false) {
            MailMessageViewModel newMessage = MailMessageViewModel.Create();
            newMessage.Subject = string.Empty;
            newMessage.To = string.Empty;
            newMessage.Text = string.Empty;
            newMessage.HasAttachment = false;
            newMessage.Date = DateTime.Now;
            newMessage.From = "Me";
            newMessage.IsReply = false;
            newMessage.IsUnread = false;
            newMessage.Priority = MessagePriority.Medium;
            if(originalMessage != null) {
                newMessage.IsReply = isForward ? originalMessage.IsReply : true;
                newMessage.HasAttachment = originalMessage.HasAttachment;
                newMessage.Subject = string.Format((isForward ? "Fwd: {0}" : "Re: {0}"), originalMessage.Subject);
                newMessage.Text = isForward 
                    ? CreateForwardMessageText(originalMessage) 
                    : CreateReplyMessageText(originalMessage);
                newMessage.To = isForward ? string.Empty : CreateReplyMessageAddress(originalMessage.From);
            }

            return newMessage;
        }
        string CreateReplyMessageText(MailMessageViewModel message) {
            using(RichEditDocumentServer server = new RichEditDocumentServer()) {
                server.MhtText = message.Text;
                InsertMessageInfo(server, message);
                QuoteReplyMessage(server, message.From, message.Date);
                return server.MhtText;
            }
        }
        string CreateForwardMessageText(MailMessageViewModel message) {
            using(RichEditDocumentServer server = new RichEditDocumentServer()) {
                server.MhtText = message.Text;
                InsertMessageInfo(server, message);
                QuoteForwardMessage(server);
                return server.MhtText;
            }
        }
        static string CreateReplyMessageAddress(string value) {
            var builder = new System.Text.StringBuilder(value);
            builder.Replace(" (", "_");
            builder.Replace(')', '_');
            builder.Replace(' ', '_');
            builder.Replace('-', '_');
            return builder.ToString();
        }
        void QuoteReplyMessage(RichEditDocumentServer server, string to, DateTime originalMessageDate) {
            QuoteMessage(server);
            Document document = server.Document;
            var name = DataHelper.GetNameByEmail(to);
            string replyHeader = String.Format("Dear, {0}\n\n{1}, you wrote:\n", string.IsNullOrEmpty(name) ? to : name, originalMessageDate);
            document.InsertText(document.Range.Start, replyHeader);
        }
        void QuoteMessage(RichEditDocumentServer server) {
            Document document = server.Document;
            ParagraphCollection paragraphs = document.Paragraphs;
            foreach(Paragraph paragraph in paragraphs) {
                DocumentRange range = paragraph.Range;
                if(document.Tables.GetTableCell(range.Start) == null && !paragraph.IsInList) {
                    document.InsertHtmlText(range.Start, ">> ");
                }
            }
        }
        void QuoteForwardMessage(RichEditDocumentServer server) {
            Document document = server.Document;
            string replyHeader = "This is a forwarded message:\n\n=================Original message text===============\n";
            document.InsertText(document.Range.Start, replyHeader);
            document.AppendText("\n\n=================End of original message text===============");
        }
        void InsertMessageInfo(RichEditDocumentServer server, MailMessageViewModel message) {
            Document document = server.Document;
            var range = document.InsertHtmlText(document.Range.Start, string.Format("<br/><p/><b>From</b>: {0}<p/><b>To</b>: {1}<p/><b>Subject</b>: {2}",
                message.From,
                message.To,
                message.Subject));
            document.InsertText(range.End, "\n");
        }
        #endregion
    }
}
