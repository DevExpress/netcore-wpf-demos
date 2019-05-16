using System.Windows;
using System.Windows.Data;
using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Navigation;

namespace DevExpress.DevAV.Common.View {
    public class TileBarFocusBehavior : Behavior<TileBar> {
        static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(DevAVDbModuleDescription), typeof(TileBarFocusBehavior),
            new FrameworkPropertyMetadata(null, (d, e) => ((TileBarFocusBehavior)d).OnSelectedItemChanged(e)));
        protected override void OnAttached() {
            base.OnAttached();
            BindingOperations.SetBinding(this, SelectedItemProperty, new Binding("SelectedItem") { Source = this.AssociatedObject, Mode = BindingMode.OneWay });
        }
        protected override void OnDetaching() {
            BindingOperations.ClearBinding(this, SelectedItemProperty);
            base.OnDetaching();
        }
        void OnSelectedItemChanged(DependencyPropertyChangedEventArgs e) {
            UIElement uiElement = (e.NewValue == null) ? null : ((UIElement)(this.AssociatedObject.ItemContainerGenerator.ContainerFromItem(e.NewValue)));
            if(uiElement != null)
                uiElement.Focus();
        }
    }
}
