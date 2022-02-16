using System;
using System.Linq;
using System.Windows.Data;

namespace Tests.Converters {
    public class LastSimbolConverter: IValueConverter {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {

            if (value is string s && s != null && s.Length > 0)
                return s.Last();
            else
                return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            return default(object);
        }
    }
}
