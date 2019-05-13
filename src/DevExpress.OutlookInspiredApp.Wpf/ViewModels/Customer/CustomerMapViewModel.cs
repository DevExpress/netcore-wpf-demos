using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm;
using DevExpress.DevAV.Common;

namespace DevExpress.DevAV.ViewModels {
    public class CustomerMapViewModel : SingleObjectViewModel<Customer, long, IDevAVDbUnitOfWork> {
        CustomerStatisticsViewModel statisticsViewModel;
        CitiesMapViewModel citiesMapViewModel;

        public CustomerMapViewModel()
            : base(UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Customers) {
        }
        protected override void OnEntityChanged() {
            base.OnEntityChanged();
            StatisticsViewModel.FilterTypeChanged -= StatisticsViewModel_FilterTypeChanged;
            var addresses = new HashSet<Address>(QueriesHelper.GetDistinctStoresForPeriod(UnitOfWork.Orders, PrimaryKey).Where(x => x != null).Select(cs => cs.Address));
            this.CitiesMapViewModel.Cities = CreateCities(addresses);
            StatisticsViewModel.EntityId = PrimaryKey;
            StatisticsViewModel.SelectedAddress = this.CitiesMapViewModel.Cities.FirstOrDefault();
            StatisticsViewModel.FilterTypeChanged +=StatisticsViewModel_FilterTypeChanged;
        }
        void StatisticsViewModel_FilterTypeChanged(object sender, PeriodEventArgs e) {
            var addresses = new HashSet<Address>(QueriesHelper.GetDistinctStoresForPeriod(UnitOfWork.Orders, PrimaryKey).Where(x => x != null).Select(cs => cs.Address));
            this.CitiesMapViewModel.Cities = CreateCities(addresses, e.Period);
        }
        List<CityViewModel> CreateCities(HashSet<Address> addresses, Period period = Period.Lifetime) {
            var newCities = new List<CityViewModel>();
            foreach(var city in addresses.Select(a => CityViewModel.Create(a, UnitOfWork))) {
                city.Sales = QueriesHelper.GetSaleMapItemsByCustomerAndCity(UnitOfWork.OrderItems, PrimaryKey, city.Address.City, period).GroupBy(mi => mi.Product).Select(gr => new SalesViewModel(gr.Key.Name, gr.CustomSum(mi => mi.Total)));
                string address = city.Address.City;
                if(!newCities.Any(c => c.Address.City == address))
                    newCities.Add(city);
            }
            return newCities;
        }
        public CitiesMapViewModel CitiesMapViewModel {
            get {
                if(citiesMapViewModel == null)
                    citiesMapViewModel = CitiesMapViewModel.Create();
                return citiesMapViewModel;
            }
        }
        public CustomerStatisticsViewModel StatisticsViewModel {
            get {
                if(statisticsViewModel == null)
                    statisticsViewModel = CustomerStatisticsViewModel.Create();
                return statisticsViewModel;
            }
        }
        public override void Delete() {
            IMessageBoxService messageBoxService = citiesMapViewModel.GetRequiredService<IMessageBoxService>();
            messageBoxService.ShowMessage("To ensure data integrity, the Customers module doesn't allow records to be deleted. Record deletion is supported by the Employees module.", "Delete Customer", MessageButton.OK);
        }
        protected override string GetTitle() {
            return "Customer Sales Map - DevAV";
        }
    }
}