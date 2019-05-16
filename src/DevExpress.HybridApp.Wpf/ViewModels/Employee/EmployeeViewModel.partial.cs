using System;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    partial class EmployeeViewModel {
        EmployeeContactsViewModel contacts;

        string firstName;
        string lastName;

        public override void OnLoaded() {
            base.OnLoaded();
            firstName = Entity.FirstName;
            lastName = Entity.LastName;
        }

        protected override bool SaveCore() {
            if(Entity.FirstName != firstName || Entity.LastName != lastName)
                Entity.FullName = Entity.FirstName + " " + Entity.LastName;
            return base.SaveCore();
        }
        public void ShowMailMerge() {
            var mailMergeViewModel = MailMergeViewModel<Employee, object>.Create(UnitOfWorkFactory, getRepositoryFunc, this.Entity.Id);
            DocumentManagerService.CreateDocument("EmployeeMailMergeView", mailMergeViewModel, null, this).Show();
        }
        public void ShowProfile() {
            Logger.Log("HybridApp: Create Report : Employee: Profile");

            DocumentManagerService.CreateDocument("ReportPreview", ReportPreviewViewModel.Create(ReportInfoFactory.EmployeeProfile(Entity)), null, this).Show();
        }
        public void ShowMeeting() {
            MessageBoxService.ShowMessage(string.Format("Schedule meeting with {0}?", Entity.FullName), "Meeting", MessageButton.YesNoCancel);
        }
        public EmployeeContactsViewModel Contacts {
            get {
                if(contacts == null)
                    contacts = EmployeeContactsViewModel.Create().SetParentViewModel(this);
                return contacts;
            }
        }
        protected override void OnEntityChanged() {
            base.OnEntityChanged();
            Contacts.Entity = Entity;
            if(Entity != null)
                Logger.Log(string.Format("HybridApp: Edit Employee: {0}",
                    string.IsNullOrEmpty(Entity.FullName) ? "<New>" : Entity.FullName));
        }
        protected override string GetTitle() {
            return Entity.FullName;
        }
        IDocumentManagerService DocumentManagerService { get { return this.GetRequiredService<IDocumentManagerService>("FrameDocumentUIService"); } }
    }
}
