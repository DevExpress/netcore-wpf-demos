using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Media;

namespace DevExpress.DevAV {
    public class CategoryColorConverter : IValueConverter {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return (value == null || string.IsNullOrEmpty(new Regex(@"^#*").Match(value.ToString()).Value)) ? 
                Colors.Transparent : (Color)ColorConverter.ConvertFromString(value.ToString());
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value == null ? null : value.ToString();
        }
    }
}
