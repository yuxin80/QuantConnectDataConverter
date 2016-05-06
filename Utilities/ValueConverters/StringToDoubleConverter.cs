using System;
using System.Globalization;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double doubleValue = System.Convert.ToDouble(value);
                return doubleValue;
            }
            catch
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dValue = System.Convert.ToDouble(value);

            if (Math.Abs(dValue * 1000) < 1 && dValue != 0)
                return dValue.ToString("E2");
            return dValue.ToString("0.###");
        }
    }
}
