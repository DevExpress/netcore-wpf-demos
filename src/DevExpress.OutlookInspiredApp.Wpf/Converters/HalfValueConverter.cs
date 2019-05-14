using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DevExpress.DevAV {
    public class HalfValueConverter : IValueConverter {
        public bool NegativeValue { get; set; }
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if(!(value is double))
                return value;
            var result = ((double)value) / 2;
            if(NegativeValue)
                result *= -1;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
