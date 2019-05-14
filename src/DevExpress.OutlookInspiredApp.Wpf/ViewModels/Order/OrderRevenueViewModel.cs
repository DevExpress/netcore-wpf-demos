using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DevExpress.DevAV.Reports;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraReports.UI;

namespace DevExpress.DevAV.ViewModels {
    public class OrderRevenueViewModel : IDocumentContent {
        public static OrderRevenueViewModel Create(IEnumerable<OrderItem> selectedItems, RevenueReportFormat format) {
            return ViewModelSource.Create(() => new OrderRevenueViewModel(selectedItems, format));
        }
        protected OrderRevenueViewModel(IEnumerable<OrderItem> selectedItems, RevenueReportFormat format) {
            Format = format;
            SelectedItems = selectedItems;
        }
        public void Close() {
            if(DocumentOwner != null)
                DocumentOwner.Close(this);
        }
        protected IDocumentManagerService DocumentManagerService { get { return this.GetService<IDocumentManagerService>(); } }

        protected IDocumentOwner DocumentOwner { get; private set; }

        public RevenueReportFormat Format { get; set; }
        public IEnumerable<OrderItem> SelectedItems { get; set; }
        public virtual XtraReport Report { get; set; }

        public virtual void OnLoaded() {
            Logger.Log(string.Format("OutlookInspiredApp: Create Report : Sales: Revenue{0}", Format.ToString()));
            Report = CreateReport();
            Report.CreateDocument();
        }

        public void ShowDesigner() {
           var viewModel = ReportDesignerViewModel.Create(CloneReport(Report));
            var doc = DocumentManagerService.CreateDocument("ReportDesignerView", viewModel, null, this);
            doc.Title = CreateTitle();
            doc.Show();

            if(viewModel.IsReportSaved) {
                Report = CloneReport(viewModel.Report);
                Report.CreateDocument();
                viewModel.Report.Dispose();
            }
        }

        XtraReport CloneReport(XtraReport report) {
            var clonedReport = CloneReportLayout(report);
            InitReport(clonedReport);
            return clonedReport;
        }
        void InitReport(XtraReport report) {
            report.DataSource = SelectedItems;
            report.Parameters["paramOrderDate"].Value = true;
        }
        XtraReport CreateReport() {
            return Format == RevenueReportFormat.Summary ? ReportFactory.SalesRevenueReport(SelectedItems, true) :
                ReportFactory.SalesRevenueAnalysisReport(SelectedItems, true);
        }
        string CreateTitle() {
            return string.Format("DevAV - {0}", Format == RevenueReportFormat.Analysis ? "Revenue Analysis" : "Revenue");
        }
        static XtraReport CloneReportLayout(XtraReport report) {
            using(var stream = new MemoryStream()) {
                report.SaveLayoutToXml(stream);
                stream.Position = 0;
                return XtraReport.FromStream(stream, true);
            }
        }
        #region IDocumentContent
        void IDocumentContent.OnClose(CancelEventArgs e) {
            Report.Dispose();
        }
        void IDocumentContent.OnDestroy() { }
        IDocumentOwner IDocumentContent.DocumentOwner {
            get { return DocumentOwner; }
            set { DocumentOwner = value; }
        }
        object IDocumentContent.Title { get { return CreateTitle(); } }
        #endregion
    }
    public enum RevenueReportFormat {
        Summary,
        Analysis
    }
}
