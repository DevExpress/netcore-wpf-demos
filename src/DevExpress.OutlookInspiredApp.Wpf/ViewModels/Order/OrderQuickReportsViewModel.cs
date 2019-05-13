using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.DevAV.Reports;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataModel;
using DevExpress.Mvvm.POCO;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace DevExpress.DevAV.ViewModels {
    public class OrderQuickReportsViewModel: IDocumentContent {
        public static OrderQuickReportsViewModel Create(Order selectedOrder, ReportFormat format) {
            return ViewModelSource.Create(() => new OrderQuickReportsViewModel(selectedOrder, format));
        }
        protected OrderQuickReportsViewModel(Order selectedOrder, ReportFormat format) {
            Format = format;
            SelectedOrder = selectedOrder;
        }
        public void Close() {
            if(DocumentOwner != null)
                DocumentOwner.Close(this);
        }
        protected IDocumentOwner DocumentOwner { get; private set; }

        public virtual Order SelectedOrder { get; set; }
        public ReportFormat Format { get; set; }

        public virtual Stream DocumentStream { get; set; }
        public virtual Tuple<IDevAVDbUnitOfWork, Order> DocumentDataSource { get; set; }

        public virtual void OnLoaded() {
            var documentStream = new MemoryStream();
            var report = ReportFactory.SalesInvoice(SelectedOrder, true, false, false, false);            
            switch(Format) {
                case ReportFormat.Pdf:
                    report.ExportToPdf(documentStream);
                    break;
                case ReportFormat.Xls:                    
                    report.ExportToXls(documentStream);
                    break;
                case ReportFormat.Doc:
                    var options = new XtraPrinting.DocxExportOptions();
                    options.ExportMode = XtraPrinting.DocxExportMode.SingleFilePageByPage;
                    options.TableLayout = true;
                    report.ExportToDocx(documentStream, options);
                    break;
            }
            DocumentDataSource = new Tuple<IDevAVDbUnitOfWork, Order>(null, SelectedOrder);
            DocumentStream = documentStream;
        }
        #region IDocumentContent
        void IDocumentContent.OnClose(CancelEventArgs e) {
            DocumentStream.Dispose();
        }
        void IDocumentContent.OnDestroy() { }
        IDocumentOwner IDocumentContent.DocumentOwner {
            get { return DocumentOwner; }
            set { DocumentOwner = value; }
        }
        object IDocumentContent.Title { get { return string.Format("Order - {0}", SelectedOrder.InvoiceNumber); } }
        #endregion
    }
    public enum ReportFormat {
        Pdf,
        Xls,
        Doc
    }
}
