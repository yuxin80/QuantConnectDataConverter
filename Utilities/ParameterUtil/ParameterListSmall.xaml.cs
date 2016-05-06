using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace CoilSimulater.Utilities
{
    /// <summary>
    /// Interaction logic for ParameterListSmall.xaml
    /// </summary>
    public partial class ParameterListSmall : ParameterCustomControlBase
    {
        public ParameterListSmall()
            : base()
        {
            InitializeComponent();

            //var dpd = DependencyPropertyDescriptor.FromProperty(ParameterListSmall.ItemProperty, typeof(ParameterListSmall));
            //dpd.AddValueChanged(this, (e, args) =>
            //{
            //    if (!IsList)
            //    {
            //        if (Item != null)
            //            button.Content = Item.ToString();
            //    }
            //});

            var dpd = DependencyPropertyDescriptor.FromProperty(ParameterListSmall.IsListProperty, typeof(ParameterListSmall));
            dpd.AddValueChanged(this, (e, args) =>
            {
                if (IsList)
                {
                    button.Content = "Click to see the list...";
                }
                else
                {
                    button.Content = Item.ToString();
                }
            });

            dpd = DependencyPropertyDescriptor.FromProperty(ParameterListSmall.ItemProperty, typeof(ParameterListSmall));
            dpd.AddValueChanged(this, (e, args) =>
            {
                button.Content = Item.ToString();
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsList)
            {
                var control = new ParameterListGrid();
                var binding = new Binding
                {
                    Path = new PropertyPath("Items", new object[0]),
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
                    Source = this
                };
                control.SetBinding(ParameterListGrid.ObjectsProperty, binding);
                WpfHelper.ShowControlInNewDialog(control, "Default object list");
            }
            else
            {
                var control = new ParameterSettingControl
                {
                    Parameter = Item
                };
                string name = this.Item != null ? this.Item.Name : "Default name";
                WpfHelper.ShowControlInNewDialog(control, name);
                button.Content = Item.ToString();
            }
        }
    }
}
