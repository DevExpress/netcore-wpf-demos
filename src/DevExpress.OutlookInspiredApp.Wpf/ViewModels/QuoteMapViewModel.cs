using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using DevExpress.DevAV;
using DevExpress.DevAV.ViewModels;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm;
using DevExpress.DevAV.Common.ViewModel;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.DevAV.Common;
using DevExpress.Mvvm.DataModel;

namespace DevExpress.DevAV.ViewModels {
    public class QuoteMapViewModel : CollectionViewModel<Quote, long, IDevAVDbUnitOfWork> {
        public QuoteCollectionViewModel Quotes { get; private set; }
        public virtual IEnumerable<object> GrouppedMapItems { get; set; }
        public virtual Stage Stage { get; set; }
        public virtual object SelectedItem { get; set; }
        public virtual bool IsHighStage { get; set; }
        public virtual bool IsMediumStage { get; set; }
        public virtual bool IsLowStage { get; set; }
        public virtual bool IsUnlikelyStage { get; set; }

        public LinksViewModel LinksViewModel {
            get {
                if(linksViewModel == null)
                    linksViewModel = LinksViewModel.Create();
                return linksViewModel;
            }
        }

        public QuoteMapViewModel()
            : base(UnitOfWorkSource.GetUnitOfWorkFactory(), x => x.Quotes) {
            IsHighStage = true;
            UpdateMapItems();
        }
        protected override void OnFilterExpressionChanged() {
            base.OnFilterExpressionChanged();
            UpdateMapItems();
        }
        protected virtual void OnStageChanged() {
            IsHighStage = object.Equals(Stage, Stage.High);
            IsMediumStage = object.Equals(Stage, Stage.Medium);
            IsLowStage = object.Equals(Stage, Stage.Low);
            IsUnlikelyStage = object.Equals(Stage, Stage.Unlikely);
            UpdateMapItems();
        }
        protected virtual void OnIsHighStageChanged() {
            if(IsHighStage)
                Stage = Stage.High;
        }
        protected virtual void OnIsMediumStageChanged() {
            if(IsMediumStage)
                Stage = Stage.Medium;
        }
        protected virtual void OnIsLowStageChanged() {
            if(IsLowStage)
                Stage = Stage.Low;
        }
        protected virtual void OnIsUnlikelyStageChanged() {
            if(IsUnlikelyStage)
                Stage = Stage.Unlikely;
        }

        void UpdateMapItems() {
            var items = GetOpportunities(Stage, FilterExpression).GroupBy<QuoteMapItem, Address>(item => item.Address, new AddressComparer());
            if(items.Count() > 0) {
                decimal minValue = items.Min(group => group.CustomSum(quote => quote.Value));
                decimal maxValue = items.Max(group => group.CustomSum(quote => quote.Value));
                decimal dif = maxValue - minValue;
                if(dif < (decimal)0.01) dif = 1;
                GrouppedMapItems = items.Select(group => new {
                    Address = group.Key,
                    Total = group.CustomSum(item => item.Value),
                    AbsSize = (double)((group.CustomSum(item => item.Value) - minValue) / dif),
                });
            } else {
                GrouppedMapItems = Enumerable.Empty<object>();
            }
            SelectedItem = GrouppedMapItems.LastOrDefault();
        }
        public IList<QuoteMapItem> GetOpportunities(Stage stage, Expression<Func<Quote, bool>> filterExpression = null) {
            var unitOfWork = CreateUnitOfWork();
            var quotes = unitOfWork.Quotes.GetFilteredEntities(filterExpression).ActualQuotes();
            var customers = unitOfWork.Customers;
            return QueriesHelper.GetOpportunities(quotes, customers, stage).ToList();
        }
        LinksViewModel linksViewModel;
    }
    class AddressComparer : IEqualityComparer<Address> {
        public bool Equals(Address x, Address y) {
            return x.State.Equals(y.State) && x.City.Equals(y.City);
        }

        public int GetHashCode(Address obj) {
            return obj.State.GetHashCode() ^ obj.City.GetHashCode();
        }
    }
}