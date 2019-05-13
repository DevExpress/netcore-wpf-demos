using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm.UI.Interactivity;

namespace DevExpress.DevAV {
    public class TreeViewSelectedItemBehavior : Behavior<TreeView> {
        #region SelectedItem Property

        public object SelectedItem {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(TreeViewSelectedItemBehavior), new PropertyMetadata(null, (d, e) => ((TreeViewSelectedItemBehavior)d).OnSelectedItemChanged()));

        #endregion

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
            Dispatcher.BeginInvoke(new Action(OnSelectedItemChanged), DispatcherPriority.ApplicationIdle);
        }

        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
        }

        void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            if(e.NewValue is FilterItem) { 
                SelectedItem = e.NewValue;
                var name = ((FilterItem)e.NewValue).Name ?? "filter";
                Logger.Log(string.Format("OutlookInspiredApp: Change Filter: {0}", name));
            }
        }

        void OnSelectedItemChanged() {
            var selectedItem = GetAllItems().FirstOrDefault(x => x.DataContext == SelectedItem);
            if(selectedItem != null)
                selectedItem.IsSelected = true;
        }

        IEnumerable<TreeViewItem> GetAllItems() {
            if(AssociatedObject == null)
                return Enumerable.Empty<TreeViewItem>();
            return AssociatedObject.Items.Cast<TreeViewItem>().SelectMany(x => x.Items.Cast<object>().Select((y, i) => (TreeViewItem)x.ItemContainerGenerator.ContainerFromIndex(i)).Where(y => y != null));
        }
    }
}
