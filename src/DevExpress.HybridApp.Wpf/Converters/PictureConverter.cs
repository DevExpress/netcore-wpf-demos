using System;
using System.Globalization;
using System.Windows.Data;
using DevExpress.DevAV;

namespace DevExpress.DevAV {
    public class PictureConverter : IValueConverter {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Picture picture = value as Picture;
            return picture == null ? null : picture.Data;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            byte[] data = value as byte[];
            return data == null ? null : new Picture() { Data = data };
        }
    }
}
