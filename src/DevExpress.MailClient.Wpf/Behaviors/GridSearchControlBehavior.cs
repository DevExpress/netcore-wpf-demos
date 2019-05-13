using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;

namespace DevExpress.MailClient.Behaviors {
    public class GridSearchControlBehavior : Behavior<GridViewBase> {
        #region Dependency Properties
        public static readonly DependencyProperty NullTextProperty =
            DependencyProperty.Register("NullText", typeof(string), typeof(GridSearchControlBehavior), new PropertyMetadata(string.Empty,
                (d, e) => ((GridSearchControlBehavior)d).UpdateSearchControl()));

        public string NullText {
            get { return (string)GetValue(NullTextProperty); }
            set { SetValue(NullTextProperty, value); }
        }
        #endregion

        SearchControl searchControl;

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.Loaded += OnGridViewLoaded;
            if(AssociatedObject.IsLoaded)
                OnGridViewLoaded(AssociatedObject, null);
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnGridViewLoaded;
            searchControl = null;
        }
        void OnGridViewLoaded(object sender, RoutedEventArgs e) {
            searchControl = AssociatedObject.SearchControl;
            UpdateSearchControl();
        }
        void UpdateSearchControl() {
            if(searchControl == null)
                return;
            BaseEdit edit = (BaseEdit)LayoutHelper.FindElementByName(searchControl, "editor");
            edit.NullText = NullText;
        }
    }
}
