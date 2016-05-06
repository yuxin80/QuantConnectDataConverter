using CoilSimulater.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
    /// Interaction logic for FileConverterStepTwo.xaml
    /// </summary>
    public partial class FileConverterStepTwo : UserControl
    {
        private FileConvertStepOne m_PreviousStep;

        private const string c_NoneItem = "None";

        public static DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(DataSet), typeof(FileConverterStepTwo));

        public DataSet Data
        {
            get { return GetValue(DataProperty) as DataSet; }
            set { SetValue(DataProperty, value); }
        }

        public static DependencyProperty ConfigurationProperty = DependencyProperty.Register("Configuration", typeof(ConverterParameters), typeof(FileConverterStepTwo));

        public ConverterParameters Configuration
        {
            get { return GetValue(ConfigurationProperty) as ConverterParameters; }
            set { SetValue(ConfigurationProperty, value); }
        }

        private List<ComboBox> m_Comboboxes = null;

        public FileConverterStepTwo(FileConvertStepOne previousStep)
        {
            InitializeComponent();
            m_Comboboxes = new List<ComboBox>(new[] {
                cmbTimeColumn,
                cmbOpenColumn,
                cmbMaxColumn,
                cmbMinColumn,
                cmbCloseColumn,
                cmbQuantColumn
            });
            m_PreviousStep = previousStep;

            var dpd = DependencyPropertyDescriptor.FromProperty(DataProperty, typeof(FileConverterStepTwo));
            dpd.AddValueChanged(this, (obj, args) =>
            {
                UpdateCombobox();
            });
        }

        private void UpdateCombobox()
        {
            if (Data == null)
            {
                ClearAllCombobox();
                return;
            }
            var table = Data.Tables["DefaultTable"];
            List<string> columns = table.Columns.OfType<DataColumn>().Select(i => i.ColumnName).ToList();
            columns.Insert(0, c_NoneItem);
            foreach (var cmb in m_Comboboxes)
            {
                cmb.Items.Clear();
                foreach (var item in columns)
                    cmb.Items.Add(item);

                cmb.SelectedIndex = 0;
            }
        }

        private void ClearAllCombobox()
        {
            foreach (var cmb in m_Comboboxes)
                cmb.Items.Clear();
        }

        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            List<string> mapping = new List<string>();
            mapping.Add(cmbTimeColumn.SelectedItem.ToString());
            mapping.Add(cmbOpenColumn.SelectedItem.ToString());
            mapping.Add(cmbMaxColumn.SelectedItem.ToString());
            mapping.Add(cmbMinColumn.SelectedItem.ToString());
            mapping.Add(cmbCloseColumn.SelectedItem.ToString());
            var quant = cmbQuantColumn.SelectedItem.ToString();
            if (quant != c_NoneItem)
                mapping.Add(quant);
            LeanDataWriter ldw = new LeanDataWriter
            {
                Data = Data,
                HeaderMapping = mapping,
                Parameters = Configuration
            };

            string diagnose;
            if(!ldw.SaveData(out diagnose))
            {
                MessageBox.Show(diagnose, "Error during saving");
            }

            var win = WpfHelper.GetParentWindow(m_PreviousStep);
            if (win != null)
                win.Close();

            var currentWin = WpfHelper.GetParentWindow(this);
            currentWin.Close();
        }

        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            var win = WpfHelper.GetParentWindow(m_PreviousStep);
            win.Show();
            var currentWin = WpfHelper.GetParentWindow(this);
            currentWin.Close();
        }
    }
}
