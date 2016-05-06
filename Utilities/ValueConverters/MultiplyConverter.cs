using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class MultiplyConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            ConstructorInfo constructor = null;
            foreach (var constr in targetType.GetConstructors())
            {
                var parameters = constr.GetParameters();
                if (parameters.Length == 0)
                {
                    constructor = constr;
                    break;
                }
            }
            if (constructor == null) return null;
            dynamic result = System.Convert.ChangeType(constructor.Invoke(new object[0]), targetType);
            foreach (var v in values)
            {
                dynamic tmp = System.Convert.ChangeType(v, targetType);
                result = result * tmp;
            }
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
