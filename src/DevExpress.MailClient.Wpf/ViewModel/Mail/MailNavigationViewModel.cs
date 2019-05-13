using DevExpress.MailClient.Helpers;
using DevExpress.Mvvm.POCO;
using System.Collections.Generic;

namespace DevExpress.MailClient.ViewModel {
    public class MailNavigationViewModel : NavigationViewModelBase {
        public static MailNavigationViewModel Create() {
            return ViewModelSource.Create(() => new MailNavigationViewModel());
        }

        public IList<IMailFolderDescription> Folders { get; private set; }
        public virtual IMailFolderDescription CurrentFolder { get; set; }

        protected MailNavigationViewModel() : base(ModuleType.Mail) {
            HeaderViewModel = HeaderViewModel.Create();
            HeaderViewModel.Init(this);
        }

        protected override void Initialize() {
            Header = "Mail";
            Icon = FilePathHelper.GetDXImageUri("Mail/Mail_32x32");
            FillFolders();
            CurrentFolder = GetFolderByFolderDescription(MessageFolderName.All, MessageType.Inbox, Folders);
        }
        protected override void InitializeInDesignMode() {
            ContentViewModel = MailViewModel.Create();
        }
        protected override void OnContentViewModelChanged(object oldValue) {
            if(oldValue != null)
                (oldValue as MailViewModel).AssignMailFolders(null);
            (ContentViewModel as MailViewModel).SetCurrentFolder(CurrentFolder);
            (ContentViewModel as MailViewModel).AssignMailFolders(Folders);
        }
        protected void OnCurrentFolderChanged() {
            if(ContentViewModel == null)
                return;

            (ContentViewModel as MailViewModel).SetCurrentFolder(CurrentFolder);
        }
        void FillFolders() {
            Folders = new List<IMailFolderDescription>();
            MailFolderViewModel mainFolder = MailFolderViewModel.Create("Mr.Brooks", FilePathHelper.GetAppImageUri("FoldersIcons/Customer"), MessageFolderName.Root, MessageType.Inbox);
            MailFolderViewModel mainInbox = MailFolderViewModel.Create("Inbox", FilePathHelper.GetAppImageUri("FoldersIcons/Inbox"), MessageFolderName.All, MessageType.Inbox);
            mainInbox.SubFolders.Add(MailFolderViewModel.Create("Management", FilePathHelper.GetAppImageUri("FoldersIcons/Management"), MessageFolderName.Management, MessageType.Inbox));
            mainInbox.SubFolders.Add(MailFolderViewModel.Create("IT", FilePathHelper.GetAppImageUri("FoldersIcons/IT"), MessageFolderName.IT, MessageType.Inbox));
            mainInbox.SubFolders.Add(MailFolderViewModel.Create("Sales", FilePathHelper.GetAppImageUri("FoldersIcons/Sales"), MessageFolderName.Sales, MessageType.Inbox));
            mainInbox.SubFolders.Add(MailFolderViewModel.Create("Engineering", FilePathHelper.GetAppImageUri("FoldersIcons/Engineering"), MessageFolderName.Engineering, MessageType.Inbox));
            mainFolder.SubFolders.Add(mainInbox);
            mainFolder.SubFolders.Add(MailFolderViewModel.Create("Sent Items", FilePathHelper.GetAppImageUri("FoldersIcons/Sent"), MessageFolderName.All, MessageType.Sent));
            mainFolder.SubFolders.Add(MailFolderViewModel.Create("Deleted Items", FilePathHelper.GetAppImageUri("FoldersIcons/Deleted"), MessageFolderName.All, MessageType.Deleted));
            mainFolder.SubFolders.Add(MailFolderViewModel.Create("Drafts", FilePathHelper.GetAppImageUri("FoldersIcons/Draft"), MessageFolderName.All, MessageType.Draft));
            Folders.Add(mainFolder);
        }
        IMailFolderDescription GetFolderByFolderDescription(MessageFolderName name, MessageType type, IEnumerable<IMailFolderDescription> folders) {
            foreach(IMailFolderDescription folder in folders) {
                if((folder.Folder == name) && (folder.Type == type))
                    return folder;

                if(folder.GetSubFolders() != null) {
                    IMailFolderDescription subFolder = GetFolderByFolderDescription(name, type, folder.GetSubFolders());
                    if(subFolder != null)
                        return subFolder;
                }
            }
            return null;
        }
    }
}
