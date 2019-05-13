using DevExpress.DevAV;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DevExpress.MailClient.ViewModel {

    public class ContactItemViewModel : ICloneable {
        public static readonly ContactItemViewModel Empty = new ContactItemViewModel();
        public static ContactItemViewModel Create() {
            return ViewModelSource.Create(() => new ContactItemViewModel());
        }
        public static ContactItemViewModel Create(Action<ContactItemViewModel> objectInitializer) {
            var result = Create();
            objectInitializer(result);
            return result;
        }

        public virtual NameViewModel Name { get; set; }
        public virtual ImageSource Photo { get; set; }
        public virtual AddressViewModel Address { get; set; }
        public virtual string Email { get; set; }
        public virtual string Phone { get; set; }
        public virtual DateTime BirthDate { get; set; }
        public virtual string Notes { get; set; }
        public virtual PersonPrefix Prefix { get; set; }

        protected ContactItemViewModel() {
            Name = NameViewModel.Create();
            Address = AddressViewModel.Create();
        }

        [Command(false)]
        public void Assign(ContactItemViewModel contact) {
            this.Name.Assign(contact.Name);
            this.Photo = contact.Photo;
            this.Address.Assign(contact.Address);
            this.Email = contact.Email;
            this.Phone = contact.Phone;
            this.BirthDate = contact.BirthDate;
            this.Prefix = contact.Prefix;
            this.Notes = contact.Notes;
        }

        #region ICloneable
        [Command(false)]
        public object Clone() {
            return Create(x => {
                x.Address = (AddressViewModel)this.Address.Clone();
                x.BirthDate = this.BirthDate;
                x.Email = this.Email;
                x.Prefix = this.Prefix;
                x.Name = (NameViewModel)this.Name.Clone();
                x.Photo = this.Photo;
                x.Phone = this.Phone;
                x.Notes = this.Notes;
            });
        }
        #endregion
    }
}
