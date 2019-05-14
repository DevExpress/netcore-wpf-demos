using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DevAV;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    public class CustomerStatisticsViewModel : StatisticsViewModelBase {
        public static CustomerStatisticsViewModel Create() {
            return ViewModelSource.Create(() => new CustomerStatisticsViewModel());
        }
        protected CustomerStatisticsViewModel() : base() { }
        protected override IEnumerable<SalesViewModel> GetActualStatistics(IDevAVDbUnitOfWork unitOfWork) {
            return QueriesHelper.GetSaleMapItemsByCustomerAndCity(unitOfWork.OrderItems, EntityId, SelectedAddress.Address.City, FilterType).GroupBy(mi => mi.Product).Select(gr => new SalesViewModel(gr.Key.Name, gr.CustomSum(mi => mi.Total)));
        }
    }
}
