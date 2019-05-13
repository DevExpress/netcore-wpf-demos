using System;
using System.ComponentModel;
using DevExpress.MailClient.Helpers;
using DevExpress.Mvvm.POCO;

namespace DevExpress.MailClient.ViewModel {
    public class AppointmentViewModel {
        public static AppointmentViewModel Create() {
            return ViewModelSource.Create(() => new AppointmentViewModel());
        }

        #region Props
        public virtual int? EventType { get; set; }
        public virtual DateTime? StartDate { get; set; }
        public virtual DateTime? EndDate { get; set; }
        public virtual bool? AllDay { get; set; }
        public virtual string Subject { get; set; }
        public virtual string Location { get; set; }
        public virtual string Description { get; set; }
        public virtual int? Status { get; set; }
        public virtual int? Label { get; set; }
        public virtual string RecurrenceInfo { get; set; }
        public virtual string ReminderInfo { get; set; }
        public virtual string ContactInfo { get; set; }
        #endregion

        protected AppointmentViewModel() {}
    }
}
