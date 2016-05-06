using System;
using System.Globalization;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class RadToDegreeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var conv1 = new RadToDegreeConverter();
            var result = conv1.Convert(value, typeof(double), parameter, culture);
            var conv2 = new DoubleToStringConverter();
            result = conv2.Convert(result, typeof(string), parameter, culture);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var conv1 = new DoubleToStringConverter();
            var result = conv1.ConvertBack(value, typeof(double), parameter, culture);
            var conv2 = new DegreeToRadConverter();
            result = conv2.Convert(result, typeof(double), parameter, culture);
            return result;
        }
    }
}
