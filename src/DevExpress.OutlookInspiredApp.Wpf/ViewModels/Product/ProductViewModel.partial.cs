using DevExpress.DevAV.Common.ViewModel;
using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm;

namespace DevExpress.DevAV.ViewModels {
    partial class ProductViewModel {
        public override void Delete() {
            MessageBoxService.ShowMessage("To ensure data integrity, the Products module doesn't allow records to be deleted. Record deletion is supported by the Employees module.", "Delete Product", MessageButton.OK);
        }
    }
}
