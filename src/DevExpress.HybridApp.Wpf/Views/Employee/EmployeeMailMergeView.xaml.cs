using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Bars;
using DevExpress.XtraRichEdit;
using System.Windows.Data;
using System;
using System.Globalization;
using System.Collections;
using System.Windows.Markup;
using DevExpress.Xpf.RichEdit;
using DevExpress.Images;

namespace DevExpress.DevAV.Views {
    public partial class EmployeeMailMergeView : UserControl {
        public EmployeeMailMergeView() {
            InitializeComponent();
        }
        void Button_Click(object sender, System.Windows.RoutedEventArgs e) {
            ((IRichEditControl)richEdit).Print();
        }
        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            richEdit.Views.PrintLayoutView.AllowDisplayLineNumbers = false;
        }
        const double additionOffset = 137;
        void richEdit_PopupMenuShowing(object sender, Xpf.RichEdit.PopupMenuShowingEventArgs e) {
            radialMenu.Items.Clear();
            var items = Validate(e.Menu.Items);
            foreach (var item in items) {
                radialMenu.Items.Add(item);
            }
            radialMenu.IsOpen = true;
            e.Menu.Items.Clear();
        }
        const int maxItemsInRadialmenu = 8;
        List<BarItem> Validate(CommonBarItemCollection items) {
            var filteredItems = items.Where(x => x is BarItem).Select(x => x as BarItem).Where(x => x.IsEnabled && x.IsVisible && !(x is BarItemSeparator)).ToList();
            UpdateImages(filteredItems);
            if (filteredItems.Count <= maxItemsInRadialmenu)
                return filteredItems;
            var firstLevelItems = filteredItems.Where(i => i is BarSubItem).ToList();
            var anotherItems = filteredItems.Where(i => !(i is BarSubItem)).ToList();
            int additionCount = maxItemsInRadialmenu - 1 - firstLevelItems.Count;
            var firstLevelAnotherItems = anotherItems.Take(additionCount).ToList();
            anotherItems.RemoveRange(0, additionCount);
            var secondLevelItems = anotherItems;
            firstLevelItems.AddRange(firstLevelAnotherItems);
            var popupMenu = new PopupMenu();
            foreach (var item in secondLevelItems) {
                popupMenu.Items.Add(item);
            }
            firstLevelItems.Add(new BarSplitButtonItem() { PopupControl = popupMenu, Content = "Actions" });
            return firstLevelItems.ToList();
        }
        void UpdateImages(IEnumerable<BarItem> filteredItems) {
            foreach (var item in filteredItems) {
                if (item.Content == null)
                    continue;
                string path;
                if (namesAndPaths.TryGetValue(item.Content.ToString(), out path)) {
                    item.LargeGlyph = GetImage(path);
                    item.GlyphSize = GlyphSize.Large;
                } else { 
                
                }
                if (item is BarSubItem)
                    UpdateImages((item as BarSubItem).Items.Where(i => i is BarItem).Select(i => i as BarItem));
            }
        }
        Dictionary<string, string> namesAndPaths = new Dictionary<string, string>() { 
            { "Toggle Field Codes", "ToggleFieldCodes_32x32.png" },
            { "Update Field", "UpdateField_32x32.png" },
            { "Paste", "Paste_32x32.png" },
            { "Increase Indent", "IndentIncrease_32x32.png" },
            { "Decrease Indent", "IndentDecrease_32x32.png" },
            { "Bookmark...", "Bookmark_32x32.png" },
            { "Paragraph...", "Paragraph_32x32.png" },
            { "Font...", "Font_32x32.png" },
            { "Hyperlink...", "Hyperlink_32x32.png" },
            { "Table Properties...", "TableProperties_32x32.png" },
            { "AutoFit", "TableAutoFitWindow_32x32.png" },
            { "Cell Alignment", "AlignMiddleCenter_32x32.png" },
            { "Split Cells...", "SplitTableCells_32x32.png" },
            { "Delete Cells...", "DeleteTableCells_32x32.png" },
            { "Insert", "InsertTable_32x32.png" },
            { "Insert Left", "InsertTableColumnsToTheLeft_32x32.png" },
            { "Insert Right", "InsertTableColumnsToTheRight_32x32.png" },
            { "Insert Above", "InsertTableRowsAbove_32x32.png" },
            { "Insert Below", "InsertTableRowsBelow_32x32.png" },
            { "Insert Cells", "InsertTableCells_32x32.png" },
            { "AutoFit Contents", "TableAutoFitContents_32x32.png" },
            { "AutoFit Window", "TableAutoFitWindow_32x32.png" },
            { "Fixed Column Width", "TableAutoFitContents_32x32.png" },
        };
        System.Windows.Media.ImageSource GetImage(string name) {
            var stream = ImageResourceCache.Default.GetResourceByFileName(name, Utils.Design.ImageType.Colored);
            if (stream == null)
                return null;

            using (stream) {
                var res = new BitmapImage();
                res.BeginInit();
                res.StreamSource = stream;
                res.EndInit();
                return res;
            }
        }
    }
}
