using System;
using System.Linq;
using System.Windows;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System.Collections.Generic;

namespace DevExpress.DevAV.Common.View {
    public class ExpandSelectedRowBehavior : Behavior<GridControl> {
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
            AssociatedObject.ItemsSourceChanged += OnAssociatedObjectItemsSourceChanged;
            AssociatedObject.FilterChanged += OnAssociatedObjectFilterChanged;
            SelectAndExpandRow();
        }
        void OnAssociatedObjectFilterChanged(object sender, RoutedEventArgs e) {
            SelectAndExpandRow();
        }
        void OnAssociatedObjectItemsSourceChanged(object sender, ItemsSourceChangedEventArgs e) {
            SelectAndExpandRow();
        }
        void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e) {
            SelectAndExpandRow();
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
            AssociatedObject.ItemsSourceChanged -= OnAssociatedObjectItemsSourceChanged;
            AssociatedObject.FilterChanged -= OnAssociatedObjectFilterChanged;
        }
        void SelectAndExpandRow(int rowHandle = 0) {
            if(!AssociatedObject.IsLoaded)
                return;
            if(AssociatedObject.View != null)
                AssociatedObject.View.FocusedRowHandle = rowHandle;
            AssociatedObject.ExpandMasterRow(rowHandle);
        }
    }
}
