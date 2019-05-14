using System.Collections.Generic;
using System.Windows.Controls;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Grid;

namespace DevExpress.DevAV.Views {
    public partial class TaskCollectionView : UserControl {
        public TaskCollectionView() {
            InitializeComponent();
        }
        void EmployeeTasks_ShowFilterPopup(object sender, FilterPopupEventArgs e) {
            if(e.Column.FieldName != "Status") return;
            List<object> statusFilterItems = new List<object>() { new CustomComboBoxItem() {
                DisplayValue = "(All)",
                EditValue = new CustomComboBoxItem()
            },
            new CustomComboBoxItem() {
                DisplayValue = "Complete",
                EditValue = CriteriaOperator.Parse("[Status] = ##Enum#DevExpress.DevAV.EmployeeTaskStatus,Completed#")
            },
            new CustomComboBoxItem() {
                DisplayValue = "Incomplete",
                EditValue = CriteriaOperator.Parse("Not([Status] = ##Enum#DevExpress.DevAV.EmployeeTaskStatus,Completed#)")
            }};
            e.ComboBoxEdit.ItemsSource = statusFilterItems;
        }
    }
}