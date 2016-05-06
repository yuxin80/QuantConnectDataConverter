using System;
using System.Globalization;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dValue = System.Convert.ToInt32(value);

            return dValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double intValue = System.Convert.ToInt32(value);
            return intValue;
        }
    }

    public class LongToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dValue = System.Convert.ToInt64(value);

            return dValue.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double intValue = System.Convert.ToInt64(value);
            return intValue;
        }
    }
}
