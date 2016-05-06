using System;
using System.Globalization;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class HalfValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (double)value;
            return -v / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = (double)value;
            return -v * 2;
        }
    }
}
