using CoilSimulater.Utilities;
using CoilSimulater.Utilities.AsciiParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataHandler
{
    /// <summary>
    /// Interaction logic for FileConvertStepOne.xaml
    /// </summary>
    public partial class FileConvertStepOne : UserControl
    {
        public static DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(FileParser), typeof(FileConvertStepOne));

        public FileParser Parameters
        {
            get { return GetValue(ParametersProperty) as FileParser; }
            set { SetValue(ParametersProperty, value); }
        }

        public static DependencyProperty ConfigurationProperty = DependencyProperty.Register("Configuration", typeof(ConverterParameters), typeof(FileConvertStepOne));

        public ConverterParameters Configuration
        {
            get { return GetValue(ConfigurationProperty) as ConverterParameters; }
            set { SetValue(ConfigurationProperty, value); }
        }

        public FileConvertStepOne()
        {
            InitializeComponent();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            var data = Parameters.ParseFile();

            FileConverterStepTwo next = new FileConverterStepTwo(this);

            WpfHelper.ShowControlInNewWindow(next, "Map columns");
            next.Data = data;
            next.Configuration = Configuration;

            var win = WpfHelper.GetParentWindow(this);
            win.Hide();
        }
    }
}
