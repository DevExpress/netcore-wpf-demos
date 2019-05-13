using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.DevAV;
using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    public class CustomerAnalysisViewModel : IDocumentContent {
        IDevAVDbUnitOfWork unitOfWork;

        public static CustomerAnalysisViewModel Create() {
            return ViewModelSource.Create(() => new CustomerAnalysisViewModel());
        }
        protected CustomerAnalysisViewModel() {
            unitOfWork = UnitOfWorkSource.GetUnitOfWorkFactory().CreateUnitOfWork();
        }
        public IEnumerable<CustomersAnalysis.Item> GetSalesReport() {
            return CustomersAnalysis.GetSalesReport(unitOfWork);
        }
        public IEnumerable<CustomersAnalysis.Item> GetSalesData() {
            return CustomersAnalysis.GetSalesData(unitOfWork);
        }
        public IEnumerable<string> GetStates(IEnumerable<StateEnum> states) {
            return QueriesHelper.GetStateNames(unitOfWork.States, states);
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
