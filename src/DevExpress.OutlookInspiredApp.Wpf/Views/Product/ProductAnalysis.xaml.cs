using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.DevAV.ViewModels;
using DevExpress.Spreadsheet;

namespace DevExpress.DevAV.Views.Product {
    public partial class ProductAnalysis : UserControl {
        public ProductAnalysis() {
            InitializeComponent();
            SubscribeEvents();
        }

        void SubscribeEvents() {
            Loaded += OnLoaded;
            spreadsheetControl.DocumentLoaded += OnDocumentLoaded;
        }

        void OnLoaded(object sender, RoutedEventArgs e) {
            ((Mvvm.ISplashScreenService)splashScreenService).ShowSplashScreen(string.Empty);
            LoadTemplate();
        }
        void LoadTemplate() {
            using(var stream = AnalysisTemplatesHelper.GetAnalysisTemplate(AnalysisTemplate.ProductSales))
                spreadsheetControl.LoadDocument(stream, DocumentFormat.Xlsm);
        }

        void OnDocumentLoaded(object sender, EventArgs e) {
            LoadData();
            ((Mvvm.ISplashScreenService)splashScreenService).HideSplashScreen();
        }
        void LoadData() {
            ProductAnalysisViewModel ViewModel = (ProductAnalysisViewModel)this.DataContext;
            spreadsheetControl.Document.BeginUpdate();
            var financialReportWorksheet = spreadsheetControl.Document.Worksheets["Financial Report"];
            var financialReportItems = ViewModel.GetFinancialReport().ToList(); // materialize
            var frProducts = financialReportItems
                .Select(i => i.ProductName)
                .Distinct()
                .OrderBy(i => i).ToList();
            financialReportWorksheet.Import(frProducts, 17, 1, true);
            var startReportsDate = financialReportItems.Min(x => x.Date);
            foreach(ProductsAnalysis.Item item in financialReportItems) {
                int rowOffset = frProducts.IndexOf(item.ProductName);
                int columnOffset = (int)(AnalysisPeriod.GetMonthOffsetFromStart(item.Date, startReportsDate) / 12);
                if (rowOffset < 0 || columnOffset < 0) continue;
                financialReportWorksheet.Cells[17 + rowOffset, 3 + columnOffset * 2].SetValue(item.Total);
            }
            var financialDataWorksheet = spreadsheetControl.Document.Worksheets["Financial Data"];
            var financialDataItems = ViewModel.GetFinancialData().ToList(); // materialize
            foreach(ProductsAnalysis.Item item in financialDataItems) {
                int rowOffset = AnalysisPeriod.GetMonthOffsetFromStart(item.Date, startReportsDate) - 1;
                int columnOffset = GetColumnIndex(item.ProductCategory);
                if (rowOffset < 0 || columnOffset < 0) continue;
                financialDataWorksheet.Cells[6 + rowOffset, 3 + columnOffset].SetValue(item.Total);
            }
            spreadsheetControl.Document.Worksheets.ActiveWorksheet = financialReportWorksheet;
            spreadsheetControl.Document.EndUpdate();
        }
        int GetColumnIndex(ProductCategory category) {
            switch(category) {
                case ProductCategory.Televisions:
                    return 0;
                case ProductCategory.Monitors:
                    return 1;
                case ProductCategory.VideoPlayers:
                    return 2;
                case ProductCategory.Automation:
                    return 3;
                default:
                    return -1;
            }
        }
    }
}
