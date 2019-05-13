using System;
using System.ComponentModel;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    public class ReportPreviewViewModel : IDocumentContent {
        public static ReportPreviewViewModel Create(IReportInfo reportInfo) {
            return ViewModelSource.Create(() => new ReportPreviewViewModel(reportInfo));
        }

        IReportInfo reportInfo;

        protected ReportPreviewViewModel(IReportInfo reportInfo) {
            this.reportInfo = reportInfo;
        }
        public void OnLoaded() {
            this.GetRequiredService<IReportService>().ShowReport(reportInfo);
        }
        public void Close() {
            if(DocumentOwner != null)
                DocumentOwner.Close(this);
        }
        protected IDocumentOwner DocumentOwner { get; private set; }
        #region IDocumentContent
        void IDocumentContent.OnClose(CancelEventArgs e) { }
        void IDocumentContent.OnDestroy() { }
        IDocumentOwner IDocumentContent.DocumentOwner {
            get { return DocumentOwner; }
            set { DocumentOwner = value; }
        }
        object IDocumentContent.Title { get { return null; } }
        #endregion
    }
}