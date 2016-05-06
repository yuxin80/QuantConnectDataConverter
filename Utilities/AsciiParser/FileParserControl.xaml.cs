using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace CoilSimulater.Utilities.AsciiParser
{
    /// <summary>
    /// Interaction logic for FileParserControl.xaml
    /// </summary>
    public partial class FileParserControl : UserControl
    {
        public static DependencyProperty ParametersProperty = DependencyProperty.Register("Parameters", typeof(FileParser), typeof(FileParserControl));

        public FileParser Parameters
        {
            get { return GetValue(ParametersProperty) as FileParser; }
            set
            {
                var param = Parameters;
                if (param != null && param != value)
                    param.PropertyChanged -= Parameters_PropertyChanged;

                SetValue(ParametersProperty, value);
            }
        }

        public FileParserControl()
        {
            InitializeComponent();

            var dpd = DependencyPropertyDescriptor.FromProperty(ParametersProperty, typeof(FileParserControl));
            dpd.AddValueChanged(this, (obj, args) =>
            {
                UpdateControls();
                Parameters.PropertyChanged += Parameters_PropertyChanged;
            });
        }

        private void Parameters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            var firstFiftyLines =Parameters==null? null : Parameters.GetSamples();

            if (firstFiftyLines != null)
                txtPreview.Text = firstFiftyLines.Aggregate((first,second)=> first + System.Environment.NewLine+second);

            var data = Parameters.ParseFile();
            if(data!= null)
            {
                dataPreview.ItemsSource = data.Tables["DefaultTable"].DefaultView;
            }
        }
    }
}
