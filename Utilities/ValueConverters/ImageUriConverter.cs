using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CoilSimulater.Utilities.ValueConverters
{
    public class ImageUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri((string)value, UriKind.Absolute);
                bitmap.EndInit();
                return bitmap;
            }
            catch (Exception ex)
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(@"/CoilSimulater.Utilities;component/Images/Default_small.png", UriKind.Relative);
                bitmap.EndInit();
                return bitmap;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Uri("");
        }
    }
}
