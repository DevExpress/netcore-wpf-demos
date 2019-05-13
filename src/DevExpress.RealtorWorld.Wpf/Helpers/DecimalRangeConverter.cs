using System;
using System.Globalization;
using System.Windows.Data;

namespace DevExpress.RealtorWorld.Xpf.Helpers {
    public class DecimalRangeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            decimal? i = value as decimal?;
            return i == null ? 0M : (decimal)i;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            decimal? i = value as decimal?;
            if(i != null) return (decimal)i;
            double? f = value as double?;
            return f == null ? 0M : (decimal)(double)f;
        }
    }
}
