using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.RealtorWorld.Xpf.DataModel;
using DevExpress.RealtorWorld.Xpf.Helpers;
using DevExpress.Mvvm;

namespace DevExpress.RealtorWorld.Xpf.ViewModel {
    public sealed class MortgageRateRepositoryViewModel : ViewModel, IDocumentContent {
        static DateTime? savedTimeRangeMinValue;
        static DateTime? savedTimeRangeMaxValue;
        readonly IUnitOfWorkFactory unitOfWorkFactory;
        IRealtorWorldUnitOfWork unitOfWork;
        DateTime? timeRangeMinValue;
        DateTime? timeRangeMaxValue;

        public MortgageRateRepositoryViewModel() : this(UnitOfWorkSource.GetUnitOfWorkFactory(IsInDesignMode)) { }
        public MortgageRateRepositoryViewModel(IUnitOfWorkFactory unitOfWorkFactory) {
            this.unitOfWorkFactory = unitOfWorkFactory;
            Refresh();
            TimeRangeMaxValue = savedTimeRangeMaxValue ?? new DateTime(2011, 10, 8);
            TimeRangeMinValue = savedTimeRangeMinValue ?? TimeRangeMaxValue - new TimeSpan(525, 0, 0, 0);
        }
        IDocumentOwner IDocumentContent.DocumentOwner { get; set; }
        object IDocumentContent.Title { get { return null; } }
        void IDocumentContent.OnClose(CancelEventArgs e) {
            savedTimeRangeMaxValue = TimeRangeMaxValue;
            savedTimeRangeMinValue = TimeRangeMinValue;
        }
        void IDocumentContent.OnDestroy() { }

        public IEnumerable<MortgageRate> Entities {
            get {
                unitOfWork.MortgageRates.Get().Load();
                return unitOfWork.MortgageRates.Local;
            }
        }
        public DateTime? TimeRangeMinValue {
            get { return timeRangeMinValue; }
            set { SetProperty(ref timeRangeMinValue, value, () => TimeRangeMinValue); }
        }
        public DateTime? TimeRangeMaxValue {
            get { return timeRangeMaxValue; }
            set { SetProperty(ref timeRangeMaxValue, value, () => TimeRangeMaxValue); }
        }
        void Refresh() {
            unitOfWork = unitOfWorkFactory.CreateUnitOfWork();
            RaisePropertyChanged(() => Entities);
        }
    }
}
