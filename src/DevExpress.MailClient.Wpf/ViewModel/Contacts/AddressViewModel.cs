using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System;

namespace DevExpress.MailClient.ViewModel {
    public class AddressViewModel : ICloneable {
        public static AddressViewModel Create(string addressString = null) {
            return ViewModelSource.Create(() => new AddressViewModel(addressString));
        }

        public virtual string AddressLine { get; set; }
        public virtual string State { get; set; }
        public virtual string City { get; set; }
        public virtual string Zip { get; set; }

        protected AddressViewModel(string addressString = null) {
            ParseAddress(addressString);
        }

        public override string ToString() {
            return string.Format("{0}{1}{2}{3}{4}", AddressLine, Environment.NewLine, GetFormatString(City, true), GetFormatString(State, false), GetFormatString(Zip, false));
        }
        [Command(false)]
        public void Assign(AddressViewModel address) {
            this.AddressLine = address.AddressLine;
            this.State = address.State;
            this.City = address.City;
            this.Zip = address.Zip;
        }
        string GetFormatString(string name, bool addComma) {
            if(string.IsNullOrEmpty(name))
                return string.Empty;
            return string.Format(addComma ? "{0}, " : "{0} ", name);
        }
        void ParseAddress(string addressString) {
            if(string.IsNullOrWhiteSpace(addressString))
                return;

            try {
                string[] lines = addressString.Split(',');
                this.AddressLine = lines[0].Trim();
                this.City = lines[1].Trim();
                this.State = lines[2].Trim().Substring(0, 2);
                string temp = lines[2].Trim();
                this.Zip = temp.Substring(3, temp.Length - 3);
            } catch { }
        }

        #region ICloneable
        [Command(false)]
        public object Clone() {
            var result = Create();
            result.Assign(this);
            return result;
        }
        #endregion
    }
}
