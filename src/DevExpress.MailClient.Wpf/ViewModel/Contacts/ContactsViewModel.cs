using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm;

namespace DevExpress.MailClient.ViewModel {
    public enum ContactsViewType { TableView, TableViewByName, TableViewByState, CardView }

    public class ContactsViewModel : ContentViewModelBase<ContactItemViewModel> {
        public static ContactsViewModel Create() {
            return ViewModelSource.Create(() => new ContactsViewModel());
        }

        public virtual ContactsViewType CurrentViewType { get; set; }
        public virtual ContactItemViewModel CurrentContact { get; set; }
        protected internal IList<string> Cities { get { return Items.Select(x => x.Address.City).OrderBy(s => s).Distinct().ToList(); } }
        protected internal IList<string> States { get { return Items.Select(x => x.Address.State).OrderBy(s => s).Distinct().ToList(); } }

        protected ContactsViewModel()
            : base(ModuleType.Contacts) {
            CurrentContact = Items.FirstOrDefault();
        }

        public void CreateNewContact() {
            EditContactCore(ContactItemViewModel.Create(), true);
        }
        public void EditContact() {
            EditContactCore((ContactItemViewModel)CurrentContact.Clone(), false);
        }
        public bool CanEditContact() {
            return IsContactActive();
        }
        public void DeleteContact() {
            Items.Remove(CurrentContact);
            ItemsSource.Remove(CurrentContact);
        }
        public bool CanDeleteContact() {
            return IsContactActive();
        }
        public void SetTableView() {
            CurrentViewType = ContactsViewType.TableView;
        }
        public void SetTableViewByName() {
            CurrentViewType = ContactsViewType.TableViewByName;
        }
        public void SetTableViewByState() {
            CurrentViewType = ContactsViewType.TableViewByState;
        }
        public void SetCardView() {
            CurrentViewType = ContactsViewType.CardView;
        }
        void EditContactCore(ContactItemViewModel contact, bool isNew) {
            ContactEditViewModel editModel = ContactEditViewModel.Create(contact, Cities, States);
            if(this.GetService<IDialogService>().ShowDialog(MessageButton.OKCancel, isNew ? "New contact" : contact.Name.FullName, editModel) == MessageResult.OK) {
                if(isNew) {
                    Items.Add(editModel.Contact);
                    ItemsSource.Add(editModel.Contact);
                    CurrentContact = editModel.Contact;
                } else
                    CurrentContact.Assign(editModel.Contact);
            }
        }
        bool IsContactActive() {
            return CurrentContact != null;
        }
    }
}
