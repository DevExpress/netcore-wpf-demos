using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core.Native;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Controls;

namespace DevExpress.DevAV {
    public class HorizontalScrollingOnMouseWheelBehavior : Behavior<ListView> {
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += OnPreviewMouseWheel;
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.PreviewMouseWheel -= OnPreviewMouseWheel;
        }
        void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            var scrollBar = (ScrollBar)LayoutHelper.FindElementByName(AssociatedObject, "PART_HorizontalScrollBar");
            if(e.Delta > 0)
                ScrollBar.LineLeftCommand.Execute(null, scrollBar);
            else if(e.Delta < 0) {
                ScrollBar.LineRightCommand.Execute(null, scrollBar);
            }
        }
    }
}