using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.DevAV.Reports.Spreadsheet;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Spreadsheet;
using DevExpress.Xpf.Spreadsheet.Extensions.Internal;

namespace DevExpress.DevAV {
    public class InvoiceEditBehavior : Behavior<SpreadsheetControl> {
        public static readonly DependencyProperty SpreadsheetDataSourceProperty =
            DependencyProperty.Register("SpreadsheetDataSource", typeof(Tuple<IDevAVDbUnitOfWork, Order>), typeof(InvoiceEditBehavior), new PropertyMetadata(null, (d, e) => ((InvoiceEditBehavior)d).InitSpreadsheet()));

        public Tuple<IDevAVDbUnitOfWork, Order> SpreadsheetDataSource {
            get { return (Tuple<IDevAVDbUnitOfWork, Order>)GetValue(SpreadsheetDataSourceProperty); }
            set { SetValue(SpreadsheetDataSourceProperty, value); }
        }

        IDevAVDbUnitOfWork unitOfWork;
        InvoiceHelper invoiceHelper;
        Order order;

        protected override void OnAttached() {
            base.OnAttached();
            Subscribe();
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            Unsubscribe();
        }

        void OnLoaded(object sender, RoutedEventArgs e) {
            InitSpreadsheet();
        }
        void InitSpreadsheet() {
            if(AssociatedObject == null || !AssociatedObject.IsLoaded)
                return;
            LoadInvoiceTemplate();
            ParseDataSource();
            CreateInvoiceHelper();
        }

        void OnPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            var cell = AssociatedObject.GetCellFromPoint(e.GetPosition(AssociatedObject).ToDrawingPoint());
            invoiceHelper.OnPreviewMouseLeftButton(cell);
        }
        void CellValueChanged(object sender, XtraSpreadsheet.SpreadsheetCellEventArgs e) {
            invoiceHelper.CellValueChanged(sender, e);
        }
        void OnSelectionChanged(object sender, EventArgs e) {
            invoiceHelper.SelectionChanged();
        }

        void CreateInvoiceHelper() {
            OrderCollections collections = new OrderCollections();
            EditActions actions = new EditActions();

            if(unitOfWork != null) {
                actions.ActivateEditor = () => {
                    var activeSheet = AssociatedObject.ActiveWorksheet;
                    if(activeSheet.Name == CellsHelper.InvoiceWorksheetName
                        && activeSheet.CustomCellInplaceEditors.GetCustomCellInplaceEditors(activeSheet.Selection).Any())
                        AssociatedObject.OpenCellEditor(XtraSpreadsheet.CellEditorMode.Edit);
                };
                actions.CloseEditor = () => {
                    if(AssociatedObject.IsCellEditorActive)
                        AssociatedObject.CloseCellEditor(DevExpress.XtraSpreadsheet.CellEditorEnterValueMode.Cancel);
                };
                actions.GetCustomerStores = x => LoadCustomerStores(x);
                actions.CreateOrderItem = () => unitOfWork.OrderItems.Create(false);
                actions.AddOrderItem = x => { unitOfWork.OrderItems.Add(x); };
                actions.RemoveOrderItem = x => { unitOfWork.OrderItems.Remove(x); };
                actions.IsDefaultActions = false;
                collections = CreateOrderCollections();
            }
            invoiceHelper = new InvoiceHelper(AssociatedObject.Document, new Tuple<OrderCollections, Order>(collections, order), actions);
        }

        void OnCustomCellEdit(object sender, SpreadsheetCustomCellEditEventArgs e) {
            if(!e.ValueObject.IsText || unitOfWork == null)
                return;
            var editorInfo = CellsHelper.FindEditor(e.ValueObject.TextValue);
            if(editorInfo != null)
                e.EditSettings = CreateSpinEditSettings(editorInfo.MinValue, editorInfo.MaxValue, editorInfo.Increment);
        }

        void LoadInvoiceTemplate() {
            using(var stream = InvoiceHelper.GetInvoiceTemplate())
                AssociatedObject.LoadDocument(stream);
        }
        void OnProtectionWarning(object sender, System.ComponentModel.HandledEventArgs e) {
            e.Handled = true;
        }
        void Subscribe() {
            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.CustomCellEdit += OnCustomCellEdit;
            AssociatedObject.SelectionChanged += OnSelectionChanged;
            AssociatedObject.CellValueChanged += CellValueChanged;
            AssociatedObject.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            AssociatedObject.ProtectionWarning += OnProtectionWarning;

        }
        void Unsubscribe() {
            AssociatedObject.Loaded -= OnLoaded;
            AssociatedObject.CustomCellEdit -= OnCustomCellEdit;
            AssociatedObject.SelectionChanged -= OnSelectionChanged;
            AssociatedObject.CellValueChanged -= CellValueChanged;
            AssociatedObject.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            AssociatedObject.ProtectionWarning -= OnProtectionWarning;
        }

        OrderCollections CreateOrderCollections() {
            var collections = new OrderCollections();
            collections.Customers = unitOfWork.Customers;
            collections.Products = unitOfWork.Products;
            collections.Employees = unitOfWork.Employees;
            collections.CustomerStores = LoadCustomerStores(order.CustomerId);
            return collections;
        }
        IEnumerable<CustomerStore> LoadCustomerStores(long? customerId) {
            return unitOfWork.CustomerStores.Where(x => x.CustomerId.Value == customerId.Value);
        }
        void ParseDataSource() {
            unitOfWork = SpreadsheetDataSource.Item1;
            order = SpreadsheetDataSource.Item2;
        }
        static SpinEditSettings CreateSpinEditSettings(int minValue, int maxValue, int increment) {
            SpinEditSettings settings = new SpinEditSettings();
            settings.MinValue = minValue;
            settings.MaxValue = maxValue;
            settings.Increment = increment;
            settings.IsFloatValue = false;
            return settings;
        }
    }
}
