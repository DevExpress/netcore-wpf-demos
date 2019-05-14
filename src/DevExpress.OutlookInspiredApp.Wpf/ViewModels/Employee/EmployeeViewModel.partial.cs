using System.Linq;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;

namespace DevExpress.DevAV.ViewModels {
    partial class EmployeeViewModel {
        EmployeeContactsViewModel contacts;
        EmployeeQuickLetterViewModel quickLetter;
        LinksViewModel linksViewModel;
        public EmployeeContactsViewModel Contacts {
            get {
                if(contacts == null) {
                    contacts = EmployeeContactsViewModel.Create().SetParentViewModel(this);
                }
                return contacts;
            }
        }
#if DXCORE3
        public CollectionViewModelBase<EmployeeTask, EmployeeTask, long, IDevAVDbUnitOfWork> EmployeeAssignedTasksDetails {
            get {
                return GetDetailsCollectionViewModel<EmployeeViewModel, EmployeeTask, long, long?>(
                    propertyExpression: (EmployeeViewModel x) => x.EmployeeAssignedTasksDetails,
                    getRepositoryFunc: x => x.Tasks,
                    foreignKeyExpression: x => x.AssignedEmployeeId,
                    navigationExpression: x => x.AssignedEmployee);
            }
        }
#else
        EmployeeTaskDetailsCollectionViewModel employeeAssignedTasksDetails;
        public EmployeeTaskDetailsCollectionViewModel EmployeeAssignedTasksDetails {
            get {
                if(employeeAssignedTasksDetails == null) {
                    employeeAssignedTasksDetails = EmployeeTaskDetailsCollectionViewModel.Create().SetParentViewModel(this);
                }
                return employeeAssignedTasksDetails;
            }
        }
#endif
        protected override void OnEntityChanged() {
            base.OnEntityChanged();
            Contacts.Entity = Entity;
            QuickLetter.Entity = Entity;
            SetDefaultReport(ReportInfoFactory.EmployeeProfile(Entity));
#if !DXCORE3
            EmployeeAssignedTasksDetails.UpdateFilter();
#endif
            if(Entity != null)
                Logger.Log(string.Format("OutlookInspiredApp: Edit Employee: {0}", 
                    string.IsNullOrEmpty(Entity.FullName) ? "<New>" : Entity.FullName));
        }
        protected override bool TryClose() {
            var closed = base.TryClose();
            if (closed)
                DocumentManagerService.Documents.First(x => x.Content == this).DestroyOnClose = true;
            return closed;
        }
        protected override string GetTitle() {
            return Entity.FullName;
        }
        public EmployeeQuickLetterViewModel QuickLetter {
            get {
                if(quickLetter == null)
                    quickLetter = EmployeeQuickLetterViewModel.Create().SetParentViewModel(this);
                return quickLetter;
            }
        }
        public void ShowMap() {
            var mapViewModel = EmployeeCollectionViewModel.CreateEmployeeMapViewModel(Entity, destination => {
                Entity.Address = destination;
                this.RaisePropertyChanged(x => x.Entity);
            });
            DocumentManagerService.CreateDocument("NavigatorMapView", mapViewModel, null, this).Show();
        }
        protected IDocumentManagerService DocumentManagerService { get { return this.GetRequiredService<IDocumentManagerService>(); } }
        public LinksViewModel LinksViewModel {
            get {
                if(linksViewModel == null)
                    linksViewModel = LinksViewModel.Create();
                return linksViewModel;
            }
        }
        public void ShowScheduler(string title) {
            MessageBoxService.Show(@"This demo does not include integration with our WPF Scheduler Suite but you can easily introduce Outlook-inspired scheduling and task management capabilities to your apps with DevExpress Tools.",
                    title, MessageButton.OK, MessageIcon.Asterisk, MessageResult.OK);
        }
        protected override bool SaveCore() {
            if(IsNew() && string.IsNullOrEmpty(Entity.FullName))
                Entity.FullName = string.Format("{0} {1}", Entity.FirstName, Entity.LastName);
            return base.SaveCore();
        }
        public void PrintEmployeeProfile() {
            ShowReport(ReportInfoFactory.EmployeeProfile(Entity), "Profile");
        }
        public bool CanPrintEmployeeProfile() {
            return Entity != null;
        }
        public void PrintSummaryReport() {
            ShowReport(ReportInfoFactory.EmployeeSummary(Repository.ToList()), "Summary");
        }
        public void PrintDirectory() {
            ShowReport(ReportInfoFactory.EmployeeDirectory(Repository.ToList()), "Directory");
        }
        public void PrintTaskList() {
            ShowReport(ReportInfoFactory.EmployeeTaskList(UnitOfWork.Tasks.ToList()), "TaskList");
        }

        void ShowReport(IReportInfo reportInfo, string reportId) {
            ExportService.ShowReport(reportInfo);
            PrintService.ShowReport(reportInfo);
            Logger.Log(string.Format("OutlookInspiredApp: Create Report : Employee: {0}", reportId));
        }
        void SetDefaultReport(IReportInfo reportInfo) {
            if(this.IsInDesignMode()) return;
            ExportService.SetDefaultReport(reportInfo);
            PrintService.SetDefaultReport(reportInfo);
        }
        IReportService PrintService { get { return this.GetRequiredService<IReportService>("PrintService"); } }
        IReportService ExportService { get { return this.GetRequiredService<IReportService>("ExportService"); } }
    }
}