using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using System;

namespace DevExpress.MailClient.ViewModel {
    public class NameViewModel : ICloneable {
        public static NameViewModel Create() {
            return ViewModelSource.Create(() => new NameViewModel());
        }

        [BindableProperty(OnPropertyChangedMethodName = "UpdateFullName")]
        public virtual string FirstName { get; set; }
        [BindableProperty(OnPropertyChangedMethodName = "UpdateFullName")]
        public virtual string MiddleName { get; set; }
        [BindableProperty(OnPropertyChangedMethodName = "UpdateFullName")]
        public virtual string LastName { get; set; }
        public virtual string FullName { get; set; }

        protected NameViewModel() { }

        [Command(false)]
        public void Assign(NameViewModel name) {
            this.FirstName = name.FirstName;
            this.MiddleName = name.MiddleName;
            this.LastName = name.LastName;
        }
        protected void UpdateFullName() {
            FullName = string.Format("{0}{1}{2}", GetFormatString(FirstName), GetFormatString(MiddleName), GetFormatString(LastName));
        }
        string GetFormatString(string name) {
            if(string.IsNullOrEmpty(name))
                return string.Empty;
            return string.Format("{0} ", name);
        }

        #region ICloneable
        public object Clone() {
            var result = Create();
            result.Assign(this);
            result.FullName = FullName;
            return result;
        }
        #endregion
    }
}
