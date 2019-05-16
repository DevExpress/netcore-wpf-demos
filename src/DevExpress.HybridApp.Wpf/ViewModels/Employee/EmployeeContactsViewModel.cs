using System;
using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    public class EmployeeContactsViewModel {
        public static EmployeeContactsViewModel Create() {
            return ViewModelSource.Create(() => new EmployeeContactsViewModel());
        }

        protected EmployeeContactsViewModel() { }

        public virtual Employee Entity { get; set; }
        protected IMessageBoxService MessageBoxService { get { return this.GetRequiredService<IMessageBoxService>(); } }

        public virtual List<EmployeeTask> EntityTasks { get; set; }

        public void Message() {
            MessageBoxService.Show("Send an IM to: " + Entity.Skype);
        }
        public bool CanMessage() {
            return Entity != null && !string.IsNullOrEmpty(Entity.Skype);
        }
        public void Phone() {
            MessageBoxService.Show("Phone Call: " + Entity.MobilePhone);
        }
        public bool CanPhone() {
            return Entity != null && !string.IsNullOrEmpty(Entity.MobilePhone);
        }
        public void HomeCall() {
            MessageBoxService.Show("Home Call: " + Entity.HomePhone);
        }
        public bool CanHomeCall() {
            return Entity != null && !string.IsNullOrEmpty(Entity.HomePhone);
        }
        public void MobileCall() {
            MessageBoxService.Show("Mobile Call: " + Entity.MobilePhone);
        }
        public bool CanMobileCall() {
            return Entity != null && !string.IsNullOrEmpty(Entity.MobilePhone);
        }
        public void Call() {
            MessageBoxService.Show("Call: " + Entity.Skype);
        }
        public bool CanCall() {
            return Entity != null && !string.IsNullOrEmpty(Entity.Skype);
        }
        public void VideoCall() {
            MessageBoxService.Show("Video Call: " + Entity.Skype);
        }
        public bool CanVideoCall() {
            return Entity != null && !string.IsNullOrEmpty(Entity.Skype);
        }
        public void MailTo() {
            ExecuteMailTo(MessageBoxService, Entity.Email);
        }
        public bool CanMailTo() {
            return Entity != null && !string.IsNullOrEmpty(Entity.Email);
        }
        protected virtual void OnEntityChanged() {
            this.RaiseCanExecuteChanged(x => x.Message());
            this.RaiseCanExecuteChanged(x => x.Phone());
            this.RaiseCanExecuteChanged(x => x.MobileCall());
            this.RaiseCanExecuteChanged(x => x.HomeCall());
            this.RaiseCanExecuteChanged(x => x.Call());
            this.RaiseCanExecuteChanged(x => x.VideoCall());
            this.RaiseCanExecuteChanged(x => x.MailTo());
            if(Entity == null) { 
                EntityTasks = null;
                return;
            }
            EntityTasks =
#if DXCORE3
            Entity.AssignedTasks;
#else
            Entity.AssignedEmployeeTasks;
#endif
        }
        public static void ExecuteMailTo(IMessageBoxService messageBoxService, string email) {
            try {
                Process.Start("mailto://" + email);
            }
            catch {
                if(messageBoxService != null)
                    messageBoxService.Show("Mail To: " + email);
            }
        }
    }
}