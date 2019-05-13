using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DevExpress.DevAV {
    public class AbsoluteToLocalConverter : IValueConverter {
        public double MaxValue { get; set; }
        public double MinValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if(!(value is double))
                return value;
            return (double)value * (MaxValue - MinValue) + MinValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
