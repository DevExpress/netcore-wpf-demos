using DevExpress.Data.Helpers;
using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Collections;
using DevExpress.MailClient.Helpers;
using DevExpress.Xpf.Grid;

namespace DevExpress.MailClient.View {
    public class SplitStringConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value == null ? null : SplitStringHelper.SplitPascalCaseString(value.ToString());
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
    public class EmptyPhotoConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value ?? new BitmapImage(FilePathHelper.GetAppImageUri("Contacts/Unknown-user", UriKind.RelativeOrAbsolute));
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return value;
        }
    }
    public class VisibleColumnsToMarginConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            IList columns = value as IList;
            if(columns == null) return new Thickness();
            double width = 0;
            foreach(DevExpress.Xpf.Grid.GridColumn column in columns) {
                if(column.CellTemplate == null) break;
                width += column.ActualDataWidth;
            }
            return new Thickness(width, 0, 0, 0);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    public class DoubleToGridColumnWidthConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return new GridColumnWidth((double)value);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return ((GridColumnWidth)value).Value;
        }
    }
}
