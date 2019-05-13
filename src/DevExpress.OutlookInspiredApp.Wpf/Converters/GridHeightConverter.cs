using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Markup;

namespace DevExpress.DevAV {
    public class GridHeightConverter : MarkupExtension, IMultiValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        public double SummaryDistance { get; set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {
            var commonHeight = (double)values[0];
            var panelHeight = (double)values[1];
            var height = commonHeight - panelHeight - SummaryDistance;
            return height < 0 ? 0 : height;
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
