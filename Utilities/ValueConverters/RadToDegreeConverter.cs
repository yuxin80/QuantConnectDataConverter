using System;
using System.Globalization;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class RadToDegreeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var radValue = (double)value;
            return radValue / Math.PI * 180;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double degValue = System.Convert.ToDouble(value);
            return degValue / 180 * Math.PI;
        }
    }
}
