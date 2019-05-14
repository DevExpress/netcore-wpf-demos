using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm;
using DevExpress.DevAV.Common;

namespace DevExpress.DevAV.ViewModels {
    public class ProductMapViewModel : SingleObjectViewModel<Product, long, IDevAVDbUnitOfWork> {
        CitiesMapViewModel citiesMapViewModel;
        ProductStatisticsViewModel statisticsViewModel;

        public ProductMapViewModel()
            : base(UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Products) {
        }
        protected override void OnEntityChanged() {
            base.OnEntityChanged();
            StatisticsViewModel.FilterTypeChanged -= StatisticsViewModel_FilterTypeChanged;
            var addresses = new HashSet<Address>(GetSalesStores().Select(cs => cs.Address));
            this.CitiesMapViewModel.Cities = CreateCities(addresses);
            StatisticsViewModel.EntityId = PrimaryKey;
            StatisticsViewModel.SelectedAddress = this.CitiesMapViewModel.Cities.FirstOrDefault();
            StatisticsViewModel.FilterTypeChanged += StatisticsViewModel_FilterTypeChanged;
        }
        void StatisticsViewModel_FilterTypeChanged(object sender, PeriodEventArgs e) {
            var addresses = new HashSet<Address>(GetSalesStores().Select(cs => cs.Address));
            this.CitiesMapViewModel.Cities = CreateCities(addresses, e.Period);
        }
        public IEnumerable<CustomerStore> GetSalesStores(Period period = Period.Lifetime) {
            return QueriesHelper.GetSalesStoresForPeriod(UnitOfWork.Orders, period);
        }
        List<CityViewModel> CreateCities(HashSet<Address> addresses, Period period = Period.Lifetime) {
            var newCities = new List<CityViewModel>();
            foreach(var city in addresses.Select(a => CityViewModel.Create(a, UnitOfWork))) {
                city.Sales = GetSalesByCity(city.Address.City, period).GroupBy(mi => mi.Customer).Select(gr => new SalesViewModel(gr.Key.Name, gr.CustomSum(mi => mi.Total)));
                string address = city.Address.City;
                if(!newCities.Any(c => c.Address.City == address))
                    newCities.Add(city);
            }
            return newCities;
        }
        IEnumerable<MapItem> GetSalesByCity(string city, Period period = Period.Lifetime) {
            return QueriesHelper.GetSaleMapItemsByCity(UnitOfWork.OrderItems, Entity.Id, city, period);
        }
        public CitiesMapViewModel CitiesMapViewModel {
            get {
                if(citiesMapViewModel == null)
                    citiesMapViewModel = CitiesMapViewModel.Create();
                return citiesMapViewModel;
            }
        }
        public ProductStatisticsViewModel StatisticsViewModel {
            get {
                if(statisticsViewModel == null)
                    statisticsViewModel = ProductStatisticsViewModel.Create();
                return statisticsViewModel;
            }
        }
        public override void Delete() {
            IMessageBoxService messageBoxService = citiesMapViewModel.GetRequiredService<IMessageBoxService>();
            messageBoxService.ShowMessage("To ensure data integrity, the Products module doesn't allow records to be deleted. Record deletion is supported by the Employees module.", "Delete Product", MessageButton.OK);
        }
        protected override string GetTitle() {
            return "Product Sales Map - DevAV";
        }
    }
}