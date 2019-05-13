using System;
using System.Collections.Generic;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Map;

namespace DevExpress.DevAV.ViewModels {
    public class CitiesMapViewModel : MapViewModelBase {
        LinksViewModel linksViewModel;
        public static CitiesMapViewModel Create() {
            return ViewModelSource.Create(() => new CitiesMapViewModel());
        }
        protected CitiesMapViewModel() {
            Cities = new List<CityViewModel>();
        }
        public virtual List<CityViewModel> Cities { get; set; }
        public virtual HashSet<Address> Addresses { get; set; }
        public LinksViewModel LinksViewModel {
            get {
                if(linksViewModel == null)
                    linksViewModel = LinksViewModel.Create();
                return linksViewModel;
            }
        }
    }
}