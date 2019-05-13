using DevExpress.MailClient.Helpers;
using DevExpress.Mvvm.POCO;

namespace DevExpress.MailClient.ViewModel {
    public class ContactsNavigationViewModel : NavigationViewModelBase {
        public static ContactsNavigationViewModel Create() {
            return ViewModelSource.Create(() => new ContactsNavigationViewModel());
        }
        protected ContactsNavigationViewModel() : base(ModuleType.Contacts) {
            HeaderViewModel = HeaderViewModel.Create();
            HeaderViewModel.Init(this);
        }
        protected override void Initialize() {
            Header = "Contacts";
            Icon = FilePathHelper.GetDXImageUri("Mail/Contact_32x32");
        }
    }
}
