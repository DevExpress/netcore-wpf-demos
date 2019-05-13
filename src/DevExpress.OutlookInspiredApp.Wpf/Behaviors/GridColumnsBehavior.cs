using System.Linq;
using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;

namespace DevExpress.DevAV {
    public class GridColumnsBehavior : Behavior<GridControl> {
        public static readonly DependencyProperty ColumnsVisibilityProperty =
            DependencyProperty.Register("ColumnsVisibility", typeof(Visibility), typeof(GridColumnsBehavior),
            new FrameworkPropertyMetadata(Visibility.Visible, (d, e) => ((GridColumnsBehavior)d).OnColumnsVisibilityChanged(e)));
        public Visibility ColumnsVisibility {
            get { return (Visibility)GetValue(ColumnsVisibilityProperty); }
            set { SetValue(ColumnsVisibilityProperty, value); }
        }
        public string[] InvisibleColumnsName { get; set; }
        void OnColumnsVisibilityChanged(DependencyPropertyChangedEventArgs e) {
            if(AssociatedObject == null || AssociatedObject.Columns == null) return;
            if(ColumnsVisibility == Visibility.Collapsed) {
                foreach(string name in InvisibleColumnsName) {
                    var columnName = name;
                    GridColumn column = AssociatedObject.Columns.SingleOrDefault(x => x.FieldName == columnName);
                    if(column != null)
                        column.Visible = false;
                }
            } else {
                int visibleIndex = 0;
                foreach(var column in AssociatedObject.Columns) {
                    column.Visible = true;
                    column.VisibleIndex = visibleIndex++;
                }
            }
        }
        protected override void OnAttached() {
            base.OnAttached();
        }
        protected override void OnDetaching() {
            base.OnDetaching();
        }
    }
}
