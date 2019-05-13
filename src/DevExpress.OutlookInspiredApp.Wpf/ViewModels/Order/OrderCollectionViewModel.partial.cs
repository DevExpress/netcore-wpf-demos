using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Map;
using DevExpress.Mvvm.ViewModel;

namespace DevExpress.DevAV.ViewModels {
    partial class OrderCollectionViewModel : ISupportFiltering<Order>, IFilterTreeViewModelContainer<Order, long> {
        const string employeeFilterCriteriaHeader = "[EmployeeId]";

        long? selectedEntityId;
        IDevAVDbUnitOfWork orderItemsUnitOfWork;

        ViewSettingsViewModel viewSettings;

        public ViewSettingsViewModel ViewSettings {
            get {
                if(viewSettings == null)
                    viewSettings = ViewSettingsViewModel.Create(CollectionViewKind.MasterDetailView, this);
                return viewSettings;
            }
        }
        public string DefaultFilter { get { return "IsOutlookIntervalYesterday([OrderDate]) OR IsOutlookIntervalToday([OrderDate])"; } }

        public override void OnLoaded() {
            base.OnLoaded();
            if(FilterTreeViewModel == null) return;
            DeleteEmployeeFilters();
            CreateEmployeeFilters();
        }
        protected override void OnSelectedEntityChanged() {
            base.OnSelectedEntityChanged();
            SetDefaultReport(ReportInfoFactory.SalesInvoice(SelectedEntity));
        }
        protected override void OnEntitiesAssigned(Func<Order> getSelectedEntityCallback) {
            RestoreSelection();
        }
        public void PrintInvoice() {
            ShowReport(ReportInfoFactory.SalesInvoice(SelectedEntity), "Invoice");
        }
        public bool CanPrintInvoice() {
            return SelectedEntity != null;
        }
        public void PrintSummaryReport() {
            ShowReport(ReportInfoFactory.SalesOrdersSummaryReport(QueriesHelper.GetSaleSummaries(CreateUnitOfWork().OrderItems)), "OrdersSummary");
        }
        public void PrintSalesAnalysisReport() {
            ShowReport(ReportInfoFactory.SalesAnalysisReport(QueriesHelper.GetSaleAnalysis(CreateUnitOfWork().OrderItems)), "Analysis");
        }
        public void ShowRevenueReport() {
            DocumentManagerService.CreateDocument("OrderRevenueReportView",
                OrderRevenueViewModel.Create(QueriesHelper.GetRevenueReportItems(CreateUnitOfWork().OrderItems), RevenueReportFormat.Summary), null, this).Show();
        }
        public virtual void ShowRevenueAnalysisReport() {
            DocumentManagerService.CreateDocument("OrderRevenueReportView",
                OrderRevenueViewModel.Create(QueriesHelper.GetRevenueAnalysisReportItems(CreateUnitOfWork().OrderItems, SelectedEntity.StoreId.Value), RevenueReportFormat.Analysis), null, this).Show();
        }
        void ShowReport(IReportInfo reportInfo, string reportId) {
            ExportService.ShowReport(reportInfo);
            PrintService.ShowReport(reportInfo);
            Logger.Log(string.Format("OutlookInspiredApp: Create Report : Sales: {0}", reportId));
        }
        void SetDefaultReport(IReportInfo reportInfo) {
            this.GetRequiredService<IReportService>("DocumentViewerService").SetDefaultReport(reportInfo);
            if(this.IsInDesignMode()) return;
            ExportService.SetDefaultReport(reportInfo);
            PrintService.SetDefaultReport(reportInfo);
        }
        IReportService PrintService { get { return this.GetRequiredService<IReportService>("PrintService"); } }
        IReportService ExportService { get { return this.GetRequiredService<IReportService>("ExportService"); } }

        public void ShowMap() {
            Logger.Log("OutlookInspiredApp: View Orders Map");
            DocumentManagerService.CreateDocument("OrderMapView", OrderMapViewModel.Create(SelectedEntity), null, this).Show();
        }
        public bool CanShowMap() {
            return SelectedEntity != null;
        }
        public void CreateCustomFilter() {
            Messenger.Default.Send(new CreateCustomFilterMessage<Order>());
        }
        public void Paid() {
            SelectedEntity.PaymentTotal = SelectedEntity.TotalAmount;
            this.Save(SelectedEntity);
        }
        public override void New() {
            var document = DocumentManagerService.ShowNewEntityDocument<Order>(this, newOrder => InitializeNewOrder(newOrder));
            var newEntity = ((OrderViewModel)document.Content).Entity;
            if(newEntity != null) {
                selectedEntityId = newEntity.Id;
                Refresh();
            }
        }
        void InitializeNewOrder(Order order) {
            var currentUnitOfWork = UnitOfWorkFactory.CreateUnitOfWork();
            Func<Order, int> getInvoiceNumber = x => {
                int number = 0;
                int.TryParse(x.InvoiceNumber, out number);
                return number;
            };
            var maxInvoiceNumber = currentUnitOfWork.Orders.Max(getInvoiceNumber);
            order.InvoiceNumber = (maxInvoiceNumber + 1).ToString();
            order.OrderDate = DateTime.Now;

            var customer = currentUnitOfWork.Customers.First();
            order.CustomerId = customer.Id;
            order.StoreId = customer.CustomerStores.First().Id;
            order.EmployeeId = currentUnitOfWork.Employees.First().Id;
        }
        protected override void OnBeforeEntityDeleted(long primaryKey, Order entity) {
            base.OnBeforeEntityDeleted(primaryKey, entity);
            if(!entity.OrderItems.Any())
                return;
            var deletedItemKeys = entity.OrderItems.Select(x => x.Id).ToList();
            orderItemsUnitOfWork = this.UnitOfWorkFactory.CreateUnitOfWork();
            deletedItemKeys.ForEach(x => {
                var item = orderItemsUnitOfWork.OrderItems.Find(x);
                orderItemsUnitOfWork.OrderItems.Remove(item);
            });
        }
        protected override void OnEntityDeleted(long primaryKey, Order entity) {
            base.OnEntityDeleted(primaryKey, entity);
            orderItemsUnitOfWork.SaveChanges();
        }
        public override void Edit(Order projectionEntity) {
            selectedEntityId = SelectedEntity.Id;
            base.Edit(projectionEntity);
            RefreshIfNeeded();
        }
        public bool CanPaid() {
            return SelectedEntity != null && SelectedEntity.PaymentTotal < SelectedEntity.TotalAmount;
        }
        public void Refund() {
            SelectedEntity.RefundTotal = SelectedEntity.PaymentTotal;
            this.Save(SelectedEntity);
        }
        public bool CanRefund() {
            return SelectedEntity != null && SelectedEntity.RefundTotal < SelectedEntity.PaymentTotal;
        }
        public void MailTo() {
            ExecuteMailTo(MessageBoxService, SelectedEntity.Employee.Email);
        }
        protected override void OnEntitySaved(long primaryKey, Order entity) {
            base.OnEntitySaved(primaryKey, entity);
            this.UpdateSelectedEntity();
        }
        public void QuickLetter(string templateName) {
            var mailMergeViewModel = MailMergeViewModel<Order, LinksViewModel>.Create(UnitOfWorkFactory, x => x.Orders, GetSelectedEntityKey(), templateName, LinksViewModel.Create());
            DocumentManagerService.CreateDocument("OrderMailMergeView", mailMergeViewModel, null, this).Show();
        }
        public bool CanQuickLetter(string templateName) {
            return SelectedEntity != null;
        }

        public void QuickReport(ReportFormat format) {
            var quickReportsViewModel = OrderQuickReportsViewModel.Create(SelectedEntity, format);
            Logger.Log(string.Format("OutlookInspiredApp: View Order Quick Report: {0}", format.ToString()));
            DocumentManagerService.CreateDocument(string.Format("Order{0}QuickReportView", format.ToString()), quickReportsViewModel, null, this).Show();
        }
        public bool CanQuickReport(ReportFormat format) {
            return SelectedEntity != null;
        }

        long? GetSelectedEntityKey() {
            return SelectedEntity == null ? (long?)null : SelectedEntity.Id;
        }

        void CreateEmployeeFilters() {
            UnitOfWorkFactory.CreateUnitOfWork().Orders
                .Where(x => x.EmployeeId != null && x.Employee != null && !string.IsNullOrEmpty(x.Employee.FullName))
                .Select(x => new FilterInfo() { FilterCriteria = employeeFilterCriteriaHeader + "=" + x.Employee.Id, Name = "Sales by " + x.Employee.FullName })
                .Distinct().ToList()
                .ForEach(x => AddEmployeeFilter(x));
        }
        void DeleteEmployeeFilters() {
            FilterTreeViewModel.CustomFilters.Where(x => IsEmployeeFilter(x))
            .ToList()
            .ForEach(x => FilterTreeViewModel.DeleteCustomFilter(x));
        }

        void AddEmployeeFilter(FilterInfo filterInfo) {
            FilterTreeViewModel.AddCustomFilter(filterInfo.Name, Data.Filtering.CriteriaOperator.Parse(filterInfo.FilterCriteria));
        }

        bool IsEmployeeFilter(FilterItem filterItem) {
            return filterItem.FilterCriteria.LegacyToString().Contains(employeeFilterCriteriaHeader);
        }
        public static void ExecuteMailTo(IMessageBoxService messageBoxService, string email) {
            try {
                System.Diagnostics.Process.Start("mailto://" + email);
            } catch {
                if(messageBoxService != null)
                    messageBoxService.Show("Mail To: " + email);
            }
        }
        void RefreshIfNeeded() {
            if(FindEntity(selectedEntityId) != null)
                Refresh();
            else
                ClearSelectedEntityId();
        }
        void RestoreSelection() {
            var entity = FindEntity(selectedEntityId);
            if(entity != null)
                SelectedEntity = entity;
            ClearSelectedEntityId();
        }
        Order FindEntity(long? id) {
            return id.HasValue ? this.Entities.SingleOrDefault(x => x.Id == id.Value) : null;
        }

        void ClearSelectedEntityId() {
            selectedEntityId = null;
        }

        #region IFilterTreeViewModelContainer
        public virtual FilterTreeViewModel<Order, long> FilterTreeViewModel { get; set; }

        #endregion

        #region ISupportFiltering
        Expression<Func<Order, bool>> ISupportFiltering<Order>.FilterExpression {
            get { return FilterExpression; }
            set { FilterExpression = value; }
        }
        #endregion
    }
}