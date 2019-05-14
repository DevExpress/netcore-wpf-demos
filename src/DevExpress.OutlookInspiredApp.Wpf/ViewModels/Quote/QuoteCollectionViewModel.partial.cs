using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.DataModel;

namespace DevExpress.DevAV.ViewModels {
    partial class QuoteCollectionViewModel : ISupportFiltering<Quote> {
        const int NumberOfAverageQuotes = 300;

        protected override void OnInitializeInRuntime() {
            base.OnInitializeInRuntime();
            UpdateAverageQuotes();
        }

        public virtual List<Quote> AverageQuotes { get; protected set; }

        public IList<QuoteSummaryItem> GetSummaryOpportunities(DateTime start, DateTime end) {
            return QueriesHelper.GetSummaryOpportunities(CreateReadOnlyRepository().GetFilteredEntities(FilterExpression).Where(x => x.Date >= start && x.Date <= end)).ToList();
        }

        protected override void OnIsLoadingChanged() {
            base.OnIsLoadingChanged();
            if(!IsLoading) {
                UpdateAverageQuotes();
            }
        }
        void UpdateAverageQuotes() {
            AverageQuotes = QueriesHelper.GetAverageQuotes(CreateReadOnlyRepository().GetFilteredEntities(FilterExpression), NumberOfAverageQuotes);
        }

        public void CreateCustomFilter() {
            Messenger.Default.Send(new CreateCustomFilterMessage<Quote>());
        }

        public override void New() { ShowProductEditForm(); }
        public override void Edit(QuoteInfo projectionEntity) { ShowProductEditForm(); }
        void ShowProductEditForm() {
            MessageBoxService.ShowMessage(@"You can easily create custom edit forms using the 40+ controls that ship as part of the DevExpress Data Editors Library. To see what you can build, activate the Employees module.",
                "Edit Opportunities", MessageButton.OK, MessageIcon.Asterisk, MessageResult.OK);
        }
        public void ShowMap(DateTime start, DateTime end) {
            QuoteMapViewModel mapViewModel = ViewModelSource.Create(
                () => new QuoteMapViewModel() { FilterExpression = (x => x.Date > start && x.Date < end) });
            var document = this.GetRequiredService<IDocumentManagerService>().CreateDocument("QuoteMapView", mapViewModel, null, this);
            Logger.Log("OutlookInspiredApp: View Opportunities Map");
            document.Title = "DevAV - Opportunities";
            document.Show();
        }
        public override void Delete(QuoteInfo projectionEntity) {
            MessageBoxService.ShowMessage("To ensure data integrity, the Opportunities module doesn't allow records to be deleted. Record deletion is supported by the Employees module.", "Delete Opportunity", MessageButton.OK);
        }
        #region ISupportFiltering
        Expression<Func<Quote, bool>> ISupportFiltering<Quote>.FilterExpression {
            get { return FilterExpression; }
            set { FilterExpression = value; }
        }
        #endregion
    }
}