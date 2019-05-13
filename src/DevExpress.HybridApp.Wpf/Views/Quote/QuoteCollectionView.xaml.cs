using System;
using System.Windows.Controls;
using DevExpress.Xpf.PivotGrid;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Threading;

namespace DevExpress.DevAV.Views {
    public partial class QuoteCollectionView : UserControl {
        public QuoteCollectionView() {
            var culture = new CultureInfo("en-us");
            InitializeComponent();
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
        void CustomSummary(object sender, PivotCustomSummaryEventArgs e) {
            switch(e.DataField.FieldName) {
                case "Percentage":
                    decimal quoteSummary = 0;
                    decimal opportunitySummary = 0;
                    foreach(PivotDrillDownDataRow row in e.CreateDrillDownDataSource()) {
                        quoteSummary += (decimal)row["Total"];
                        opportunitySummary += (decimal)row["MoneyOpportunity"];
                    }
                    if(quoteSummary != 0)
                        e.CustomValue = 100M * opportunitySummary / quoteSummary;
                    break;
            }
        }
        void pivotGridSizeChanged(object sender, SizeChangedEventArgs e) {
            PivotGridControl pivotGrid = (PivotGridControl)sender;
            PivotGridField fieldPercentage = (pivotGrid.Fields).GetFieldByName("fieldPercentage");
            PivotGridField fieldQuote = (pivotGrid.Fields).GetFieldByName("fieldQuote");
            double estimatedWidth = e.NewSize.Width - pivotGrid.RowTreeOffset - pivotGrid.RowTreeMinWidth - fieldQuote.MinWidth - 3;
            if(estimatedWidth > fieldPercentage.MinWidth)
                fieldPercentage.Width = estimatedWidth;
        }
    }
}