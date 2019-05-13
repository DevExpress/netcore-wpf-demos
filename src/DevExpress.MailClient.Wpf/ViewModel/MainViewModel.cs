using System.Windows.Media.Imaging;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System.Text.RegularExpressions;
using DevExpress.MailClient.Helpers;
using DevExpress.Mvvm.ModuleInjection;

namespace DevExpress.MailClient.ViewModel {
    public class MainViewModel {
        public static MainViewModel Create() {
            return ViewModelSource.Create(() => new MainViewModel()); ;
        }
        protected IModuleManager Manager { get { return ModuleManager.DefaultManager; } }

        protected MainViewModel() {
#if !DXCORE3
            if(!ViewModelBase.IsInDesignMode)
                RegisterJumpList();
#endif
            MetadataLocator.Default = MetadataLocator.Create().AddMetadata<PrefixEnumWithExternalMetadata>();
        }
        [Command(false)]
        public void RegisterJumpList() {
            this.GetService<IApplicationJumpListService>().Items.AddOrReplace("Navigation", "Mail", new BitmapImage(FilePathHelper.GetDXImageUri("Mail/Mail_16x16")), OpenMail);
            this.GetService<IApplicationJumpListService>().Items.AddOrReplace("Navigation", "Calendar", new BitmapImage(FilePathHelper.GetDXImageUri("Scheduling/Today_16x16")), OpenCalendar);
            this.GetService<IApplicationJumpListService>().Items.AddOrReplace("Navigation", "Contacts", new BitmapImage(FilePathHelper.GetDXImageUri("Mail/Contact_16x16")), OpenContacts);
            this.GetService<IApplicationJumpListService>().Items.AddOrReplace("Navigation", "Tasks", new BitmapImage(FilePathHelper.GetDXImageUri("Tasks/Task_16x16")), OpenTasks);
            this.GetService<IApplicationJumpListService>().Items.AddOrReplace("Tasks", "New Mail", new BitmapImage(FilePathHelper.GetDXImageUri("Mail/NewMail_16x16")), OpenMailAndCreateNewMessage);
            this.GetService<IApplicationJumpListService>().Apply();
        }

        public void OpenMail() {
            Manager.Navigate(Regions.Documents, Modules.Mail);
        }
        public void OpenCalendar() {
            Manager.Navigate(Regions.Documents, Modules.Calendar);
        }
        public void OpenContacts() {
            Manager.Navigate(Regions.Documents, Modules.Contacts);
        }
        public void OpenTasks() {
            Manager.Navigate(Regions.Documents, Modules.Tasks);
        }
        public void ShowAbout() {
            DevExpress.Xpf.About.ShowAbout(DevExpress.Utils.About.ProductKind.DXperienceWPF);
        }
        public void Exit() {
            System.Windows.Application.Current.MainWindow.Close();
        }
        void OpenMailAndCreateNewMessage() {
            OpenMail();
            (Manager.GetRegion(Regions.Documents).GetViewModel(Modules.Mail) as MailViewModel).CreateNewMessage();
        }
    }
}
