using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class ObjectToTypeConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var converter = TypeDescriptor.GetConverter(targetType);
            return converter.ConvertFrom(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var converter = TypeDescriptor.GetConverter(targetType);
            return converter.ConvertFrom(value);
        }

        #endregion
    }
}
