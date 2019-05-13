using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    partial class EmployeeCollectionViewModel : ISupportFiltering<Employee>, IFilterTreeViewModelContainer<Employee, long> {
        IDocumentManagerService EditNoteDocumentManagerService { get { return this.GetRequiredService<IDocumentManagerService>("EditNoteDocumentManagerService"); } }

        public void ShowMailMerge() {
            var mailMergeViewModel = MailMergeViewModel<Employee, object>.Create(UnitOfWorkFactory, x => x.Employees, SelectedEntity == null ? null as long? : SelectedEntity.Id);
            DocumentManagerService.CreateDocument("EmployeeMailMergeView", mailMergeViewModel, null, this).Show();
        }
        public void ShowPrint(EmployeeReportType employeeReportType) {
            DocumentManagerService.CreateDocument("ReportPreview", ReportPreviewViewModel.Create(GetReport(employeeReportType)), null, this).Show();
        }
        public bool CanShowPrint(EmployeeReportType employeeReportType) {
            return employeeReportType != EmployeeReportType.Profile || SelectedEntity != null;
        }
        IReportInfo GetReport(EmployeeReportType reportType) {
            Logger.Log(string.Format("HybridApp: Create Report : Employees: {0}", reportType.ToString()));
            switch(reportType) {
                case EmployeeReportType.TaskList:
                    return ReportInfoFactory.EmployeeTaskList(UnitOfWorkFactory.CreateUnitOfWork().Tasks.ToList());
                case EmployeeReportType.Profile:
                    return ReportInfoFactory.EmployeeProfile(SelectedEntity);
                case EmployeeReportType.Summary:
                    return ReportInfoFactory.EmployeeSummary(Entities);
                case EmployeeReportType.Directory:
                    return ReportInfoFactory.EmployeeDirectory(Entities);
            }
            throw new ArgumentException("", "reportType");
        }
        protected override void OnEntitiesAssigned(Func<Employee> getSelectedEntityCallback) {
            base.OnEntitiesAssigned(getSelectedEntityCallback);
            if(SelectedEntity == null)
                SelectedEntity = Entities.FirstOrDefault();
        }
        public void AddTask() {
            Action<EmployeeTask> initializer = x => {
                x.AssignedEmployeeId = SelectedEntity.Id;
                x.OwnerId = SelectedEntity.Id;
            };
            EditNoteDocumentManagerService.CreateDocument("EmployeeTaskView", null, initializer, this).Show();
        }
        public bool CanAddTask() {
            return SelectedEntity != null;
        }
        public void AddNote() {
            Action<Evaluation> initializer;
            if(SelectedEntity == null)
                initializer = default(Action<Evaluation>);
             else
                initializer = (x) => x.EmployeeId = SelectedEntity.Id;        
            EditNoteDocumentManagerService.CreateDocument("EvaluationView", null, initializer, this).Show();
        }
        public bool CanAddNote() {
            return SelectedEntity != null;
        }
        public virtual FilterTreeViewModel<Employee, long> FilterTreeViewModel { get; set; }
        #region ISupportFiltering
        Expression<Func<Employee, bool>> ISupportFiltering<Employee>.FilterExpression {
            get { return FilterExpression; }
            set { FilterExpression = value; }
        }
        #endregion
    }
}