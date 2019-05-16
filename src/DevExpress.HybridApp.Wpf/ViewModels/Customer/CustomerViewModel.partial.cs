using System;
using System.Collections.Generic;
using System.Linq;

namespace DevExpress.DevAV.ViewModels {
    partial class CustomerViewModel {
        public virtual object SelectedItem { get; set; }
        protected override void OnEntityChanged() {
            base.OnEntityChanged();
            Logger.Log("HybridApp: View Customer");
        }

        public IEnumerable<object> UpdateMapItems(DateTime start, DateTime end) {
            IEnumerable<object> mapItems;
            var items = Entity.Orders.Where(x => x.OrderDate >= start && x.OrderDate <= end).GroupBy<Order, CustomerStore>(order => order.Store);
            if(items.Count() > 0) {
                decimal minimumSalesValue = items.Min<IGrouping<CustomerStore, Order>>(orders => orders.CustomSum(order => order.TotalAmount));
                decimal maximumSalesValue = items.Max<IGrouping<CustomerStore, Order>>(orders => orders.CustomSum(order => order.TotalAmount));
                decimal dif = maximumSalesValue - minimumSalesValue;
                mapItems = items.Select(group => new {
                    Address = group.Key.Address,
                    TotalSales = group.CustomSum(order => order.TotalAmount),
                    TotalOpportunities = QueriesHelper.GetQuotesTotal(UnitOfWork.Quotes, group.Key, start, end),
                    AbsSize = dif > 0 ? (double)((group.CustomSum(order => order.TotalAmount) - minimumSalesValue) / dif) : 1.0
                });
            } else
                mapItems = Enumerable.Empty<object>();
            SelectedItem = mapItems.LastOrDefault();
            return mapItems;
        }
        protected override string GetTitle() {
            return string.Format("Customer {0}", this.Entity.Name);
        }
    }
}
