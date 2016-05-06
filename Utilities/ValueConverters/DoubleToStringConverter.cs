using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dValue = System.Convert.ToDouble(value);

            if (Math.Abs(dValue * 1000) < 1 && dValue != 0)
                return dValue.ToString("E2");
            return dValue.ToString("0.###");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double doubleValue = System.Convert.ToDouble(value);
            return doubleValue;
        }
    }

    public class DoubleToStringConverterNonFormatted : IValueConverter
    {
        private string s_InputString = string.Empty;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (String.IsNullOrEmpty(s_InputString))
            {
                var dValue = System.Convert.ToDouble(value);

                return dValue.ToString("0.########");
            }
            var result = s_InputString;
            s_InputString = string.Empty;
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            s_InputString = string.Empty;
            if (value is string)
            {
                var regx = new Regex(@"\.(\d*0+)?$");
                var regxSci = new Regex(@"[eE]-?\d*$");
                string str = value.ToString();
                if (regx.IsMatch(str) || regxSci.IsMatch(str))
                    s_InputString = str;
            }
            double doubleValue = System.Convert.ToDouble(value);
            return doubleValue;
        }
    }
}
