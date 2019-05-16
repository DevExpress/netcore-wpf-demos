using System.Windows.Input;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;

namespace DevExpress.DevAV {
    public class CollectionViewBehavior : Behavior<GridControl> {
        public ICommand SortAscendingCommand { get; private set; }
        public ICommand SortDescendingCommand { get; private set; }
        public CollectionViewBehavior() {
            SortAscendingCommand = new DelegateCommand<string>(x => {
                if(AssociatedObject.Columns[x] != null && AssociatedObject != null)
                    AssociatedObject.SortBy(AssociatedObject.Columns[x], Data.ColumnSortOrder.Ascending);
            });
            SortDescendingCommand = new DelegateCommand<string>(x => {
                if(AssociatedObject.Columns[x] != null && AssociatedObject != null)
                    AssociatedObject.SortBy(AssociatedObject.Columns[x], Data.ColumnSortOrder.Descending);
            });
        }
        protected override void OnAttached() {
            base.OnAttached();
        }
        protected override void OnDetaching() {
            base.OnDetaching();
        }
    }
}