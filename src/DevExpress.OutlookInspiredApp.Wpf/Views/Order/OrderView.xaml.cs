using DevExpress.DevAV.DevAVDbDataModel;
using DevExpress.DevAV.ViewModels;
using DevExpress.Spreadsheet;
using DevExpress.Xpf.Spreadsheet;
using DevExpress.XtraRichEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.DevAV.Reports.Spreadsheet;

namespace DevExpress.DevAV.Views {
    /// <summary>
    /// Interaction logic for InvoiceEditorView.xaml
    /// </summary>
    public partial class OrderView : UserControl {
        public OrderView() {
            InitializeComponent();
        }
    }

    public class CellTemplateSelector : DataTemplateSelector {
        public DataTemplate AddOrderItemTemplate { get; set; }
        public DataTemplate DeleteOrderItemTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container) {
            var cell = ((CellData)item).Cell;
            if(IsInvoiceWorksheet(cell) && IsRangeForAddCommand(cell, GetInvoiceItems(cell)))
                return AddOrderItemTemplate;
            if(IsInvoiceWorksheet(cell) && IsRangeForDeleteCommand(cell, GetInvoiceItems(cell)))
                return DeleteOrderItemTemplate;
            return base.SelectTemplate(item, container);
        }
        bool IsRangeForDeleteCommand(Cell cell, DefinedName invoiceItems) {
            return invoiceItems != null && invoiceItems.Range != null && cell.ColumnIndex == 10 && invoiceItems.Range.RowCount > 1 &&
                cell.RowIndex >= invoiceItems.Range.TopRowIndex && cell.RowIndex <= invoiceItems.Range.BottomRowIndex;
        }
        bool IsRangeForAddCommand(Cell cell, DefinedName invoiceItems) {
            return cell.ColumnIndex == 6 && cell.RowIndex == (invoiceItems == null || invoiceItems.Range == null ? 21 : invoiceItems.Range.BottomRowIndex + 1);
        }
        DefinedName GetInvoiceItems(Cell cell) {
            return cell.Worksheet.DefinedNames.GetDefinedName("InvoiceItems");
        }
        bool IsInvoiceWorksheet(Cell cell) {
            return cell.Worksheet.Name == CellsHelper.InvoiceWorksheetName;
        }
    }
}
