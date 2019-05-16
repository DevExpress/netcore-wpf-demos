using System;
using System.Linq;
using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.DevAV;

namespace DevExpress.DevAV.ViewModels {
    partial class EmployeeTaskViewModel {
        protected override EmployeeTask CreateEntity() {
            EmployeeTask entity = base.CreateEntity();
            entity.StartDate = DateTime.Now;
            entity.DueDate = DateTime.Now + new TimeSpan(48, 0, 0);
            return entity;
        }
        protected override void OnEntityChanged() {
            base.OnEntityChanged();
            this.RaisePropertyChanged(vm => vm.ReminderTime);
            if(Entity != null)
                Logger.Log(string.Format("HybridApp: Edit Task: {0}",
                    string.IsNullOrEmpty(Entity.Subject) ? "<New>" : Entity.Subject));
        }
        public DateTime? ReminderTime {
            get {
                if(this.Entity == null || this.Entity.ReminderDateTime == null)
                    return null;
                return this.Entity.ReminderDateTime.Value;
            }
            set {
                if(this.Entity == null || value == null || this.Entity.ReminderDateTime == null)
                    return;
                DateTime reminderDateTime = (DateTime)this.Entity.ReminderDateTime;
                this.Entity.ReminderDateTime = new DateTime(reminderDateTime.Year, reminderDateTime.Month, reminderDateTime.Day, ((DateTime)value).Hour, ((DateTime)value).Minute, reminderDateTime.Second);
            }
        }
        protected override string GetTitle() {            
            return Entity.Owner != null ? Entity.Owner.FullName : string.Empty;
        }
    }
}