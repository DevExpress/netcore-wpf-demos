using System.Linq;
using DevExpress.DevAV;
using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Map;
using System.Collections.Generic;
using System;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.Mvvm.POCO;

namespace DevExpress.DevAV.ViewModels {
    public abstract class StatisticsViewModelBase {
        IDevAVDbUnitOfWork unitOfWork;

        protected StatisticsViewModelBase() {
            unitOfWork = UnitOfWorkSource.GetUnitOfWorkFactory().CreateUnitOfWork();
            FilterType = Period.Lifetime;
        }
        public event EventHandler<PeriodEventArgs> FilterTypeChanged;
        public long EntityId { get; set; }
        [BindableProperty(OnPropertyChangedMethodName = "UpdateStatistics")]
        public virtual CityViewModel SelectedAddress { get; set; }
        public void ThisMonthFilter() { FilterType = Period.ThisMonth; }
        public bool CanThisMonthFilter() { return (!object.Equals(FilterType, Period.ThisMonth)); }
        public void YTDFilter() { FilterType = Period.ThisYear; }
        public bool CanYTDFilter() { return !object.Equals(FilterType, Period.ThisYear); }
        public void LifetimeFilter() { FilterType = Period.Lifetime; }
        public bool CanLifetimeFilter() { return !object.Equals(FilterType, Period.Lifetime); }
        public virtual Period FilterType { get; set; }
        protected void OnFilterTypeChanged() {
            if(FilterTypeChanged != null)
                FilterTypeChanged(this, new PeriodEventArgs(FilterType));
            UpdateStatistics();
        }
        public virtual IEnumerable<SalesViewModel> ActualStatistics { get; set; }
        protected void UpdateStatistics() { 
            if(SelectedAddress == null)
                return;
            ActualStatistics = GetActualStatistics(this.unitOfWork);
        }
        protected abstract IEnumerable<SalesViewModel> GetActualStatistics(IDevAVDbUnitOfWork unitOfWork);
    }
    public class SalesViewModel {
        public SalesViewModel(string name, decimal total) {
            Name = name;
            Total = total;
        }
        public string Name { get; set; }
        public decimal Total { get; set; }
    }
    public class CityViewModel {
        Lazy<Crest> crest;
        Lazy<State> state;

        public static CityViewModel Create(Address address, IDevAVDbUnitOfWork unitOfWork) {
            return ViewModelSource.Create(() => new CityViewModel(address, unitOfWork));
        }
        protected CityViewModel(Address address, IDevAVDbUnitOfWork unitOfWork) {
            Address = address;
            Location = new GeoPoint(address.Latitude, address.Longitude);
            state = new Lazy<DevAV.State>(() => unitOfWork.States.First(s => s.ShortName == address.State));
            crest = new Lazy<DevAV.Crest>(() => unitOfWork.Crests.First(c => c.CityName == address.City));
        }        
        public Crest Crest { get { return crest.Value; } }
        public State State { get { return state.Value; } }
        public Address Address { get; set; }
        public GeoPoint Location { get; set; }
        public virtual bool IsVisible { get; set; }
        public virtual IEnumerable<SalesViewModel> Sales { get; set; }
        protected void OnSalesChanged() {
            IsVisible = !(Sales == null || Sales.Count() == 0);
        }
    }
    public class PeriodEventArgs : EventArgs {
        public PeriodEventArgs(Period period) {
            Period = period;
        }
        public Period Period { get; set; }
    }
}