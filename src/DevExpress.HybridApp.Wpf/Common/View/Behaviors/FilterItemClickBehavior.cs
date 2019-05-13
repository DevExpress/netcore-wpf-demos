using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevExpress.DevAV {
    public class FilterItemClickBehavior: Behavior<TileBarItem> {
        protected override void OnAttached() {
            base.OnAttached();
            this.AssociatedObject.Click += AssociatedObject_Click;
        }
        void AssociatedObject_Click(object sender, EventArgs e) {
            var filterItem = AssociatedObject.DataContext as FilterItem;
            if(filterItem == null || filterItem.Name == null)
                return;
            Logger.Log(string.Format("OutlookInspiredApp: Change Filter: {0}", filterItem.Name));
        }
        protected override void OnDetaching() {
            this.AssociatedObject.Click -= AssociatedObject_Click;
            base.OnDetaching();
        }
    }
}
