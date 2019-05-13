using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DevExpress.RealtorWorld.Xpf.Helpers {
    public class ImageSourceConverter : DependencyObject, IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return ImageSourceHelper.GetImageSource(value as byte[], Dispatcher);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotSupportedException();
        }
    }
}
