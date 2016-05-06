using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class MultiBindingDefaultConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            List<object> convertedValue = new List<object>();
            foreach (var obj in values)
            {
                convertedValue.Add(obj);
            }
            return convertedValue.ToArray();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return value as object[];
        }

        #endregion
    }
}
