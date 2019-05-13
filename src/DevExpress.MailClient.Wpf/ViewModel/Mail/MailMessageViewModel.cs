using System;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using DevExpress.MailClient.Helpers;
using System.Windows.Media;

namespace DevExpress.MailClient.ViewModel {
    public enum Categories { General = 1, Management = 2, IT = 3, Sales = 4, Support = 5, Engineering = 6, HR = 7, Design = 8 };
    public enum MessageFolderName { All, Announcements, General, Management, IT, Sales, Support, Engineering, Deleted, Custom, Root };

    public enum MessageType { Inbox, Deleted, Sent, Draft };
    public enum MessagePriority { Low, Medium, High }

    [POCOViewModel(ImplementIDataErrorInfo = true)]
    public class MailMessageViewModel {
        public static MailMessageViewModel Create() {
            return ViewModelSource.Create(() => new MailMessageViewModel());
        }

        public MessageFolderName Folder { get; set; }
        public MessageType Type { get; set; }
        public string SubjectDisplayText { get { return string.Format("{1}{0}", Subject, IsReply ? "Re: " : ""); } }

        public virtual string PlainText { get; protected set; }
        public virtual DateTime Date { get; set; }
        public virtual string From { get; set; }
        [EmailAddressValidationAttribute(true)]
        public virtual string To { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Text { get; set; }
        public virtual bool IsUnread { get; set; }
        public virtual bool IsReply { get; set; }
        public virtual bool HasAttachment { get; set; }
        public virtual MessagePriority Priority { get; set; }
        public virtual string SenderName { get; set; }
        public virtual ImageSource SenderPhoto { get; set; }
        public virtual string FullName {
            get {
                if(this.IsInDesignMode())
                    return From;
                if(string.IsNullOrEmpty(From)) return SenderName;
                return string.Format("{0} ({1})", SenderName, From);
            }
        }
        public virtual void OnFromChanged() {
            SenderName = DataHelper.GetNameByEmail(From, this.IsInDesignMode());
            SenderPhoto = DataHelper.GetPictureByEmail(From, this.IsInDesignMode());
        }

        protected MailMessageViewModel() {
            From = string.Empty;
            To = string.Empty;
            SenderName = string.Empty;
            Subject = string.Empty;
            Text = string.Empty;
            Priority = MessagePriority.Medium;
        }

        [Command(false)]
        public void Assign(MailMessageViewModel message) {
            Folder = message.Folder;
            Type = message.Type;
            Date = message.Date;
            From = message.From;
            SenderName = message.SenderName;
            To = EmailValidationHelper.NormalizeEmailsString(message.To);
            Subject = message.Subject;
            Text = message.Text;
            IsUnread = message.IsUnread;
            IsReply = message.IsReply;
            HasAttachment = message.HasAttachment;
            Priority = message.Priority;
        }
        [Command(false)]
        public void NormalizeEmails() {
            To = EmailValidationHelper.NormalizeEmailsString(To);
        }
        protected virtual void OnTextChanged() {
            if(string.IsNullOrEmpty(Text)) {
                PlainText = null;
                return;
            }
            var plainText = DataHelper.GetPlainTextFromMHT(Text).Replace("\r\n", " ");
            PlainText = plainText.Length > 200 ? string.Format("{0}...", plainText.Remove(197)) : plainText;
        }
    }
    class EmailAddressValidationAttribute : ValidationAttribute {
        public bool MultipleEmails { get; private set; }

        public EmailAddressValidationAttribute(bool multipleEmails) {
            this.MultipleEmails = multipleEmails;
        }

        public override bool IsValid(object value) {
            string mails = (string)value;
            if(string.IsNullOrWhiteSpace(mails))
                return false;

            if(!MultipleEmails)
                return EmailValidationHelper.IsEmailValid(mails);

            bool result = true;
            bool hasValues = false;

            foreach(string email in EmailValidationHelper.ExtractEmails(mails)) {
                result &= EmailValidationHelper.IsEmailValid(email);
                hasValues = true;
            }
            return result && hasValues;
        }
    }
}
