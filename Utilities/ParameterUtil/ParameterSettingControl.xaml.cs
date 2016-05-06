using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CoilSimulater.Utilities.ValidationRules;
using CoilSimulater.Utilities.ValueConverters;
using System.Collections.ObjectModel;

namespace CoilSimulater.Utilities
{
    /// <summary>
    /// Interaction logic for ParameterSettingDialog.xaml
    /// </summary>
    public partial class ParameterSettingControl : UserControl
    {
        public static DependencyProperty DialogNameProperty = DependencyProperty.Register("DialogName", typeof(string), typeof(ParameterSettingControl), new PropertyMetadata("Parameter setting"));

        public string DialogName
        {
            get { return GetValue(DialogNameProperty) as string; }
            set { SetValue(DialogNameProperty, value); }
        }

        public static DependencyProperty ParameterProperty = DependencyProperty.Register("Parameter", typeof(ObjectBase), typeof(ParameterSettingControl));

        public ObjectBase Parameter
        {
            get { return GetValue(ParameterProperty) as ObjectBase; }
            set { SetValue(ParameterProperty, value as ObjectBase); }
        }

        public ParameterSettingControl()
        {
            InitializeComponent();

            var dpd = DependencyPropertyDescriptor.FromProperty(ParameterProperty, this.GetType());
            dpd.AddValueChanged(this, (e, args) =>
            {
                InitializeUI();
            });
        }

        public Boolean OnApply(out string diagnose)
        {
            if (!ValidateInput())
            {
                diagnose = "There are invalid inputs. Please correct them before click Ok button.";
                return false;
            }
            diagnose = "";
            return true;
        }

        private bool ValidateInput()
        {
            return IsValid(this);
        }

        private bool IsValid(DependencyObject obj)
        {
            return !Validation.GetHasError(obj) &&
            LogicalTreeHelper.GetChildren(obj)
            .OfType<DependencyObject>()
            .All(IsValid);
        }

        private void InitializeUI()
        {
            Contents.Children.Clear();
            Contents.RowDefinitions.Clear();

            if (Parameter == null) return;

            var itemsCache = ParameterUtil.GetControlList(Parameter);

            int rowNum = 0;
            double height = 0;
            double width = 200;

            foreach (var data in itemsCache)
            {
                var name = data.First;
                var control = data.Second;
                double tmpWidth = (name.Length + 1) * 8;
                Label label = new Label();
                label.HorizontalAlignment = HorizontalAlignment.Left;
                label.VerticalAlignment = VerticalAlignment.Center;
                label.Content = string.Format("{0}:", name);
                Contents.Children.Add(label);
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, rowNum);
                control.VerticalAlignment = VerticalAlignment.Center;
                control.DataContext = Parameter;
                Contents.Children.Add(control);
                Grid.SetRow(control, rowNum);
                Grid.SetColumn(control, 1);
                width = Math.Max(tmpWidth + 100, width);
                if (control is ParameterListSmall)
                {
                    var listSmall = control as ParameterListSmall;
                    if (listSmall.IsList)
                    {
                        var binding = BindingOperations.GetBinding(listSmall, ParameterListSmall.ObjectsProperty);
                        var propertyName = binding.Path.Path;
                        var propertyInfo = Parameter.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).First(i => i.Name == propertyName);
                        var list = propertyInfo.GetValue(Parameter) as IEnumerable;
                        listSmall.Items = list;
                    }
                }
                rowNum++;
            }

            for (int i = 0; i < rowNum; i++)
            {
                Contents.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30) });
                height += 30;
            }

            this.DataContext = Parameter;
            this.Height = height + 100;
            this.MaxHeight = height + 100;
            this.MinHeight = height + 100;
        }
    }
}
