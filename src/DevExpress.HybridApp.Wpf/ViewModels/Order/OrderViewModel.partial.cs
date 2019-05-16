using System;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    partial class OrderViewModel {
        public override void OnLoaded() {
            base.OnLoaded();
            this.GetRequiredService<IReportService>().ShowReport(ReportInfoFactory.SalesInvoice(Entity));
        }
    }
}
