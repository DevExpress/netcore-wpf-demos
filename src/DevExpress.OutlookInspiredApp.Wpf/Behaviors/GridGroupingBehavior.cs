using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;

namespace DevExpress.DevAV {
    public class GridGroupingBehavior : Behavior<GridControl> {
        TableView tableView;

        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register("Group", typeof(string), typeof(GridGroupingBehavior),
            new FrameworkPropertyMetadata(null, (d, e) => ((GridGroupingBehavior)d).OnGroupChanged(e)));
        public string Group {
            get { return (string)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }
        public static readonly DependencyProperty NewItemCommandProperty =
            DependencyProperty.Register("NewItemCommand", typeof(ICommand), typeof(GridGroupingBehavior), new PropertyMetadata(null));
        public ICommand NewItemCommand {
            get { return (ICommand)GetValue(NewItemCommandProperty); }
            set { SetValue(NewItemCommandProperty, value); }
        }
        void OnGroupChanged(DependencyPropertyChangedEventArgs e) {
            if(AssociatedObject == null) return;
            if(string.IsNullOrEmpty(Group))
                AssociatedObject.ClearGrouping();
            if(AssociatedObject.Columns.GetColumnByFieldName(Group) != null)
                AssociatedObject.GroupBy(Group);
        }
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.Loaded += Loaded;
        }
        void Loaded(object sender, RoutedEventArgs e) {
            tableView = AssociatedObject.View as TableView;
            if(tableView != null) {
                tableView.FocusedRowHandleChanged -= FocusedRowHandleChanged;
                tableView.FocusedRowHandleChanged += FocusedRowHandleChanged;
            }
        }
        void FocusedRowHandleChanged(object sender, FocusedRowHandleChangedEventArgs e) {
            if(e.RowData.RowHandle.Value == GridControl.NewItemRowHandle && AssociatedObject.SelectedItems.Count == 0) {
                if(NewItemCommand != null)
                    NewItemCommand.Execute(null);
                tableView.FocusedRowHandle = -1;
            }
        }
        protected override void OnDetaching() {
            AssociatedObject.Loaded -= Loaded;
            if(tableView != null) {
                tableView.FocusedRowHandleChanged -= FocusedRowHandleChanged;
            }
            base.OnDetaching();
        }
    }
}
