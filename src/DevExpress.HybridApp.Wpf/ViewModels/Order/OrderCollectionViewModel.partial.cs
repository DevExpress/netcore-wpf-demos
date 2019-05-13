using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    partial class OrderCollectionViewModel {
        const int NumberOfAverageOrders = 200;
        public virtual List<Order> AverageOrders { get; set; }
        public virtual List<SalesInfo> Sales { get; set; }
        public virtual SalesInfo SelectedSale { get; set; }

        public void ShowPrintPreview() {
            var link = this.GetRequiredService<DevExpress.DevAV.Common.View.IPrintableControlPreviewService>().GetLink();
            this.GetRequiredService<IDocumentManagerService>("FrameDocumentUIService").CreateDocument("PrintableControlPrintPreview", PrintableControlPreviewViewModel.Create(link), null, this).Show();
        }

        protected override void OnInitializeInRuntime() {
            base.OnInitializeInRuntime();
            var unitOfWork = UnitOfWorkFactory.CreateUnitOfWork();
            Sales = QueriesHelper.GetSales(unitOfWork.OrderItems);
            SelectedSale = Sales[0];
            AverageOrders = QueriesHelper.GetAverageOrders(unitOfWork.Orders.ActualOrders(), NumberOfAverageOrders);
        }
        protected override void OnEntitiesAssigned(Func<Order> getSelectedEntityCallback) {
            base.OnEntitiesAssigned(getSelectedEntityCallback);
            if(SelectedEntity == null)
                SelectedEntity = Entities.FirstOrDefault();
        }
    }
}