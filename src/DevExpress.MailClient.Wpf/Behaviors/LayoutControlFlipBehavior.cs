using System.Linq;
using DevExpress.Xpf.LayoutControl;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.MailClient.ViewModel;
using System.Windows;
using System.Collections.Generic;

namespace DevExpress.MailClient.Behaviors {
    public class LayoutControlFlipBehavior : Behavior<LayoutControl> {
        public static readonly DependencyProperty LayoutModeProperty = 
            DependencyProperty.Register("LayoutMode", typeof(LayoutMode), typeof(LayoutControlFlipBehavior),
            new PropertyMetadata(LayoutMode.Normal, (d, e) => ((LayoutControlFlipBehavior)d).OnLayoutModeChanged()));
        public static readonly DependencyProperty OrderIndexProperty = 
            DependencyProperty.RegisterAttached("OrderIndex", typeof(int), typeof(LayoutControlFlipBehavior), new PropertyMetadata(-1));
        public static int GetOrderIndex(DependencyObject target) {
            return (int)target.GetValue(OrderIndexProperty);
        }
        public static void SetOrderIndex(DependencyObject target, int value) {
            target.SetValue(OrderIndexProperty, value);
        }

        public LayoutMode LayoutMode {
            get { return (LayoutMode)GetValue(LayoutModeProperty); }
            set { SetValue(LayoutModeProperty, value); }
        }

        void OnLayoutModeChanged() {
            if(AssociatedObject == null)
                return;

            IEnumerable<FrameworkElement> children = AssociatedObject.Children.Cast<FrameworkElement>().ToList();
            if(LayoutMode == LayoutMode.Normal)
                children = children.OrderBy(x => GetOrderIndex(x));
            else
                children = children.OrderByDescending(x => GetOrderIndex(x));

            AssociatedObject.Children.Clear();
            children.ToList().ForEach(x => {
                x.Width = double.NaN;
                x.Height = double.NaN;
                AssociatedObject.Children.Add(x);
            });
        }
    }
}
