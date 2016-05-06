using System;
using System.Globalization;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class DegreeToRadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var degValue = (double)value;
            return degValue / 180 * Math.PI;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double radValue = System.Convert.ToDouble(value);
            return radValue / Math.PI * 180;
        }
    }
}
