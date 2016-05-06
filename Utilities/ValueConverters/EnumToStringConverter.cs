using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var type = value.GetType();
            if (type.IsEnum)
            {
                return GetEnumDescription(value);
            }
            return string.Empty;
        }

        private object GetEnumDescription(object value)
        {
            FieldInfo fieldInfo = value.GetType().GetField(value.ToString());

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            foreach (var attr in attribArray)
            {
                if (attr is DescriptionAttribute)
                {
                    DescriptionAttribute attrib = attr as DescriptionAttribute;
                    return attrib.Description;
                }
            }

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Empty;
        }
    }
}
