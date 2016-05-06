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
    /// Interaction logic for ParameterListGrid.xaml
    /// </summary>
    public partial class ParameterListGrid : ParameterCustomControlBase
    {
        private Type m_ItemType = null;

        public ParameterListGrid()
        {
            InitializeComponent();

            IsList = true;

            DataContent.AutoGenerateColumns = false;

            var dpd = DependencyPropertyDescriptor.FromProperty(ObjectsProperty, this.GetType());
            dpd.AddValueChanged(this, (e, args) =>
            {
                UpdateList();
            });
        }

        private void UpdateList()
        {
            DataContent.Columns.Clear();
            try
            {
                var list = Items.Cast<ObjectBase>();
                if (list == null || list.Count() <= 0)
                {
                    AddEnd.IsEnabled = false;
                    Delete.IsEnabled = false;
                    return;
                }

                var firstObj = list.ElementAt(0);
                var controls = ParameterUtil.GetControlList(firstObj);
                m_ItemType = firstObj.GetType();

                foreach (var pair in controls)
                {
                    var control = pair.Second;
                    if (control is TextBox)
                    {
                        var column = new DataGridTextColumn
                        {
                            Header = pair.First
                        };
                        var binding = control.GetBindingExpression(TextBox.TextProperty);
                        if (binding != null)
                        {
                            var newBinding = new Binding
                            {
                                Path = binding.ParentBinding.Path,
                            };
                            foreach (var vr in binding.ParentBinding.ValidationRules)
                                newBinding.ValidationRules.Add(vr);

                            column.Binding = newBinding;
                        }
                        DataContent.Columns.Add(column);

                    }
                }

                DataContent.ItemsSource = this.Items;
            }
            catch (Exception ex)
            {
                DataContent.Visibility = Visibility.Hidden;
                var list = Items.Cast<object>();
                if (list.Count() <= 0)
                    m_ItemType = typeof(object);
                else
                {
                    var firstItem = list.ElementAt(0);
                    if (firstItem is string)
                        m_ItemType = typeof(string);
                    else
                        m_ItemType = typeof(object);
                }
                ListContent.ItemsSource = Items;

            }
        }

        private void AddEnd_Click(object sender, RoutedEventArgs e)
        {
            if (m_ItemType != null)
            {
                if (m_ItemType == typeof(ObjectBase))
                {
                    var item = Activator.CreateInstance(m_ItemType) as ObjectBase;

                    Items = Items.Cast<ObjectBase>().Concat(new[] { item });
                }
                else
                {
                    var item = Activator.CreateInstance(m_ItemType) as Object;
                    Items = Items.Cast<Object>().Concat(new[] { item });
                }
            }
            Delete.IsEnabled = true;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = DataContent.SelectedIndex;
            var list = Items.Cast<Object>();
            if (index >= 0 && index < list.Count())
            {
                var listObj = list.ToList();
                listObj.RemoveAt(index);
                Items = listObj;
            }

            if (list.Count() <= 0)
                Delete.IsEnabled = false;
        }
    }
}
