using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace DevExpress.DevAV {
    public class IntToHoursConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            decimal? intValue = value as decimal?;
            if(intValue == null) return null;
            if(intValue < 0 || intValue > 23)
                throw new ArgumentException("Incorrect value hours!", "value");
            return intValue < 13 ? string.Format("{0}AM", intValue) : string.Format("{0}PM", intValue - 12);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
