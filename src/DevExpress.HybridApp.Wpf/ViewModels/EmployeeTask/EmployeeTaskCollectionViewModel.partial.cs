using System;
using System.Linq.Expressions;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    partial class EmployeeTaskCollectionViewModel : ISupportFiltering<EmployeeTask>, IFilterTreeViewModelContainer<EmployeeTask, long> {
        public void ShowPrintPreview() {
            var link = this.GetRequiredService<DevExpress.DevAV.Common.View.IPrintableControlPreviewService>().GetLink();
            this.GetRequiredService<IDocumentManagerService>("FrameDocumentUIService").CreateDocument("PrintableControlPrintPreview", PrintableControlPreviewViewModel.Create(link), null, this).Show();
        }        
        public virtual FilterTreeViewModel<EmployeeTask, long> FilterTreeViewModel { get; set; }
        #region ISupportFiltering
        Expression<Func<EmployeeTask, bool>> ISupportFiltering<EmployeeTask>.FilterExpression {
            get { return FilterExpression; }
            set { FilterExpression = value; }
        }
        #endregion
    }
}