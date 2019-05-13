using DevExpress.Mvvm.POCO;
using System.Collections.Generic;

namespace DevExpress.MailClient.ViewModel {

    public class ContactEditViewModel {
        public static ContactEditViewModel Create(ContactItemViewModel contact, IList<string> cities, IList<string> states) {
            return ViewModelSource.Create(() => new ContactEditViewModel(contact, cities, states));
        }

        public virtual ContactItemViewModel Contact { get; set; }
        public IList<string> States { get; private set; }
        public IList<string> Cities { get; private set; }

        protected ContactEditViewModel(ContactItemViewModel contact, IList<string> cities, IList<string> states) {
            this.Contact = contact;
            this.Cities = cities;
            this.States = states;
        }
        protected ContactEditViewModel() {
            if(this.IsInDesignMode())
                InitializeInDesignMode();
        }

        void InitializeInDesignMode() {
            var contactsModel = ContactsViewModel.Create();
            Contact = contactsModel.CurrentContact;
            Cities = contactsModel.Cities;
            States = contactsModel.States;
        }
    }
}
