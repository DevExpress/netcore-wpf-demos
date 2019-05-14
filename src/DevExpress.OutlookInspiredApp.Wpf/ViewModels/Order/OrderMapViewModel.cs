using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevExpress.DevAV;
using DevExpress.DevAV.ViewModels;
using DevExpress.Map;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Map;
using System.Windows;
using System.IO;
using DevExpress.DevAV.Reports;

namespace DevExpress.DevAV.ViewModels {
    public class OrderMapViewModel: NavigatorMapViewModel<CustomerStore> {
        public static OrderMapViewModel Create(Order order) {
            return ViewModelSource.Create(() => new OrderMapViewModel(order));
        }
        protected OrderMapViewModel(Order order)
            : base(order.Store,
                  AddressHelper.DevAVHomeOffice.ToString(),
                  new GeoPoint(AddressHelper.DevAVHomeOffice.Latitude, AddressHelper.DevAVHomeOffice.Longitude),
                  order.Store.Address.ToString(),
                  new GeoPoint(order.Store.Address.Latitude, order.Store.Address.Longitude), null) {
            Order = order;
        }
        public Order Order { get; protected set; }
        public virtual string ShipmentText { get; set; }
        public virtual Stream PdfStream { get; set; }

        public override void OnLoaded() {
            base.OnLoaded();
            CreateShippingDetailPdf();
        }
        public virtual void OuUnloaded() {
            PdfStream.Dispose();
        }
        void CreateShippingDetailPdf() {
            PdfStream = new MemoryStream();
            ReportFactory.ShippingDetail(Order).ExportToPdf(PdfStream);
            ShipmentText = GetShipmentText();
        }
        string GetShipmentText() {
            switch(Order.ShipmentStatus) {
                case ShipmentStatus.Received:
                    return "Shipment Received";
                case ShipmentStatus.Transit:
                    return "Shipment in Transit";
            }
            return "Awaiting shipment";
        }
    }
}
