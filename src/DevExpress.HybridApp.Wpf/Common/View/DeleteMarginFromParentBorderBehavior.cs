using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DevExpress.Mvvm.UI.Interactivity;

namespace DevExpress.DevAV.Common.View {
    public class DeleteMarginFromParentBorderBehavior : Behavior<FrameworkElement>{
        protected override void OnAttached() {
            base.OnAttached();
            SearchBorderAndMarginReset(AssociatedObject);
        }

        void SearchBorderAndMarginReset(FrameworkElement obj) {
            while(obj != null && obj.Name != "Border") {
                obj = VisualTreeHelper.GetParent(obj) as FrameworkElement;
            }
            if(obj == null)
                return;
            obj.Margin = new Thickness(0);
        }
    }
}
