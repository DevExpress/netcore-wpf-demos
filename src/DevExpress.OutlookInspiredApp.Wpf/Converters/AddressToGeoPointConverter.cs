using System;
using System.Globalization;
using System.Windows.Data;
using DevExpress.DevAV;
using DevExpress.Map;
using DevExpress.Xpf.Map;

namespace DevExpress.DevAV {
    public class AddressToGeoPointConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var address = value as Address;
            if(address == null)
                return value;
            return new GeoPoint(address.Latitude, address.Longitude);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
