using System.Windows;
using System.Windows.Data;
using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Navigation;

namespace DevExpress.DevAV {
    public class FilterUnselectionBehavior : Behavior<TileBar> {
        bool selectFilterEnable = true;

        public static readonly DependencyProperty SelectedFilterProperty =
            DependencyProperty.Register("SelectedFilter", typeof(FilterItem), typeof(FilterUnselectionBehavior),
                new PropertyMetadata(null, (d, e) => ((FilterUnselectionBehavior)d).OnSelectedFilterChanged()));
        static readonly DependencyProperty TileBarItemInternalProperty =
            DependencyProperty.Register("TilebarItemInternal", typeof(FilterItem), typeof(FilterUnselectionBehavior),
                new PropertyMetadata(null, (d, e) => ((FilterUnselectionBehavior)d).OnTileBarItemInternalChanged()));

        public FilterItem SelectedFilter {
            get { return (FilterItem)GetValue(SelectedFilterProperty); }
            set { SetValue(SelectedFilterProperty, value); }
        }
        FilterItem TileBarItemInternal {
            get { return (FilterItem)GetValue(TileBarItemInternalProperty); }
            set { SetValue(TileBarItemInternalProperty, value); }
        }

        void OnSelectedFilterChanged() {
            if(AssociatedObject == null || AssociatedObject.ItemsSource == null || SelectedFilter == TileBarItemInternal) return;
            if(SelectedFilter == null) {
                SelectTileBarItem(null);
                return;
            }
            foreach(var item in AssociatedObject.ItemsSource)
                if(item == SelectedFilter) {
                    SelectTileBarItem(SelectedFilter);
                    return;
                }
            SelectTileBarItem(null);
        }
        void OnTileBarItemInternalChanged() {
            if(selectFilterEnable)
                SelectedFilter = TileBarItemInternal;
        }

        protected override void OnAttached() {
            base.OnAttached();
            BindingOperations.SetBinding(this, FilterUnselectionBehavior.TileBarItemInternalProperty, new Binding("SelectedItem") { Source = AssociatedObject, Mode = BindingMode.OneWay });
            OnSelectedFilterChanged();
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            BindingOperations.ClearBinding(this, FilterUnselectionBehavior.TileBarItemInternalProperty);
        }

        void SelectTileBarItem(FilterItem item) {
            selectFilterEnable = false;
            AssociatedObject.SelectedItem = item;
            selectFilterEnable = true;
        }
    }
}
