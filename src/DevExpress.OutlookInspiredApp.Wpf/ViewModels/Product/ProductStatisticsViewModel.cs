using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    public class ProductStatisticsViewModel : StatisticsViewModelBase {
        public static ProductStatisticsViewModel Create() {
            return ViewModelSource.Create(() => new ProductStatisticsViewModel());
        }
        protected ProductStatisticsViewModel() : base() { }
        protected override IEnumerable<SalesViewModel> GetActualStatistics(IDevAVDbUnitOfWork unitOfWork) {
            return QueriesHelper.GetSaleMapItemsByCity(unitOfWork.OrderItems, EntityId, SelectedAddress.Address.City, FilterType).GroupBy(mi => mi.Customer).Select(gr => new SalesViewModel(gr.Key.Name, gr.CustomSum(mi => mi.Total)));
        }
    }
}