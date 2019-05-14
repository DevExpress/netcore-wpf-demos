using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Mvvm;
using DevExpress.DevAV;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    public class ProductAnalysisViewModel : IDocumentContent {
        IDevAVDbUnitOfWork unitOfWork;

        public static ProductAnalysisViewModel Create() {
            return ViewModelSource.Create(() => new ProductAnalysisViewModel());
        }
        protected ProductAnalysisViewModel() {
            unitOfWork = UnitOfWorkSource.GetUnitOfWorkFactory().CreateUnitOfWork();
        }
        public IEnumerable<ProductsAnalysis.Item> GetFinancialReport() {
            return ProductsAnalysis.GetFinancialReport(unitOfWork);
        }
        public IEnumerable<ProductsAnalysis.Item> GetFinancialData() {
            return ProductsAnalysis.GetFinancialData(unitOfWork);
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
        object IDocumentContent.Title { get { return "DevAV - Sales Analysis"; } }
        #endregion
    }
}