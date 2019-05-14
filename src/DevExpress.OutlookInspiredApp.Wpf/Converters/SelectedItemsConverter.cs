using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Markup;

namespace DevExpress.DevAV {
    public class SelectedItemsConverter : MarkupExtension, IValueConverter {
        public override object ProvideValue(IServiceProvider serviceProvider) {
            return this;
        }
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if(value == null) return null;
            var result = new List<object>();
            foreach(var item in (List<Employee>)value)
                result.Add(item);
            return result;
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            List<Employee> result = new List<Employee>();
            if(value != null)
                foreach(object item in ((List<object>)value)) {
                    if(item as Employee != null)
                        result.Add((Employee)item);
                }
            return result;
        }
    }
}
