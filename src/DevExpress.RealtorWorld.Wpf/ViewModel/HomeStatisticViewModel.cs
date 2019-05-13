using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DevExpress.RealtorWorld.Xpf.DataModel;
using DevExpress.RealtorWorld.Xpf.Helpers;
using DevExpress.Mvvm;

namespace DevExpress.RealtorWorld.Xpf.ViewModel {
    public sealed class HomeStatisticViewModel : ViewModel, IDocumentContent {
        static DateTime? savedTimeRangeMinValue;
        static DateTime? savedTimeRangeMaxValue;
        static decimal? savedYearRangeMinValue;
        static decimal? savedYearRangeMaxValue;
        readonly IUnitOfWorkFactory unitOfWorkFactory;
        IRealtorWorldUnitOfWork unitOfWork;
        Home selectedHome;
        HomePopularityRating popularityRating;
        HomePriceStatisticData prices;
        SimilarHousesStatisticData similarHouses;
        DateTime? timeRangeMinValue;
        DateTime? timeRangeMaxValue;
        decimal yearRangeMinValue;
        decimal yearRangeMaxValue;

        public HomeStatisticViewModel() : this(UnitOfWorkSource.GetUnitOfWorkFactory(IsInDesignMode)) { }
        public HomeStatisticViewModel(IUnitOfWorkFactory unitOfWorkFactory) {
            this.unitOfWorkFactory = unitOfWorkFactory;
            Refresh();
            TimeRangeMaxValue = savedTimeRangeMaxValue ?? DateTime.Now.Date;
            TimeRangeMinValue = savedTimeRangeMinValue ?? TimeRangeMaxValue - new TimeSpan(100, 0, 0, 0);
            YearRangeMaxValue = savedYearRangeMaxValue == null ? DateTime.Now.Year + 0.6M : (decimal)savedYearRangeMaxValue;
            YearRangeMinValue = savedYearRangeMinValue == null ? YearRangeMaxValue - 6.2M : (decimal)savedYearRangeMinValue;
        }
        IDocumentOwner IDocumentContent.DocumentOwner { get; set; }
        object IDocumentContent.Title { get { return null; } }
        void IDocumentContent.OnClose(CancelEventArgs e) {
            savedTimeRangeMaxValue = TimeRangeMaxValue;
            savedTimeRangeMinValue = TimeRangeMinValue;
            savedYearRangeMaxValue = YearRangeMaxValue;
            savedYearRangeMinValue = YearRangeMinValue;
        }
        void IDocumentContent.OnDestroy() { }

        public IEnumerable<Home> Entities {
            get {
                unitOfWork.Homes.Get().Load();
                return unitOfWork.Homes.Local;
            }
        }
        public Home SelectedHome {
            get { return selectedHome; }
            set { SetProperty(ref selectedHome, value, () => SelectedHome, OnSelectedHomeChanged); }
        }
        public HomePopularityRating PopularityRating {
            get { return popularityRating; }
            private set { SetProperty(ref popularityRating, value, () => PopularityRating); }
        }
        public HomePriceStatisticData Prices {
            get { return prices; }
            private set { SetProperty(ref prices, value, () => Prices); }
        }
        public SimilarHousesStatisticData SimilarHouses {
            get { return similarHouses; }
            private set { SetProperty(ref similarHouses, value, () => SimilarHouses); }
        }
        public DateTime? TimeRangeMinValue {
            get { return timeRangeMinValue; }
            set { SetProperty(ref timeRangeMinValue, value, () => TimeRangeMinValue); }
        }
        public DateTime? TimeRangeMaxValue {
            get { return timeRangeMaxValue; }
            set { SetProperty(ref timeRangeMaxValue, value, () => TimeRangeMaxValue); }
        }
        public decimal YearRangeMinValue {
            get { return yearRangeMinValue; }
            set { SetProperty(ref yearRangeMinValue, value, () => YearRangeMinValue); }
        }
        public decimal YearRangeMaxValue {
            get { return yearRangeMaxValue; }
            set { SetProperty(ref yearRangeMaxValue, value, () => YearRangeMaxValue); }
        }

        void OnSelectedHomeChanged() {
            PopularityRating = SelectedHome == null ? null : unitOfWork.HomePopularityRatings.Find(SelectedHome.StatisticDataID);
            Prices = SelectedHome == null ? null : unitOfWork.HomePriceStatisticData.Find(SelectedHome.HomePriceStatisticDataKey);
            SimilarHouses = SelectedHome == null ? null : unitOfWork.SimilarHousesStatisticData.Find(SelectedHome.StatisticDataID);
        }
        void Refresh() {
            unitOfWork = unitOfWorkFactory.CreateUnitOfWork();
            RaisePropertyChanged(() => Entities);
            if(SelectedHome == null)
                SelectedHome = Entities.FirstOrDefault();
        }
    }
}
