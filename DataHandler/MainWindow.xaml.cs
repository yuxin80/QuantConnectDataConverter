using CoilSimulater.Utilities;
using CoilSimulater.Utilities.AsciiParser;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(ConverterParameters), typeof(MainWindow));

        public ConverterParameters Parameters
        {
            get { return GetValue(ParametersProperty) as ConverterParameters; }
            set { SetValue(ParametersProperty, value); }
        }

        public MainWindow()
        {
            Parameters = new ConverterParameters();
            InitializeComponent();
        }

        private void m_btnConvert_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Parameters.SourceFileName))
                return;

            //if (string.IsNullOrEmpty(Parameters.DestFolderName))
            //    return;

            FileParser parser = new FileParser
            {
                FileName = Parameters.SourceFileName
            };

            var control = new FileConvertStepOne();
            control.Parameters = parser;
            control.Configuration = Parameters;

            //var data = parser.ParseFile();

            //DataGrid dataGrid = new DataGrid();
            //dataGrid.ItemsSource = data.Tables["DefaultTable"].DefaultView;
            WpfHelper.ShowControlInNewWindow(control, "Step 1");
        }
    }
}
