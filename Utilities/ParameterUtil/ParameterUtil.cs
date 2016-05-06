using CoilSimulater.Utilities.Templates;
using CoilSimulater.Utilities.ValidationRules;
using CoilSimulater.Utilities.ValueConverters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace CoilSimulater.Utilities
{
    public static class ParameterUtil
    {
        public static List<Pair<string, FrameworkElement>> GetControlList(ObjectBase param)
        {
            var paramType = param.GetType();

            var itemsCache = new List<Pair<string, FrameworkElement>>();
            var properties = paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                string name = property.Name;
                double min = double.NegativeInfinity;
                double max = double.PositiveInfinity;
                var attrs = property.GetCustomAttributes(true);
                bool isparameter = false;
                Type listType = null;
                bool isParameterList = false;
                string tipMessage = string.Empty;
                IValueConverter valueConverter = null;
                string bindingBooleanProperty = string.Empty;
                Boolean isReadOnly = false;
                string listPropName = string.Empty;
                Boolean isFileName = false;
                Boolean isFolderName = false;
                foreach (var att in attrs)
                {
                    if (att is IsFileNameAttribute)
                    {
                        isFileName = true;
                    }
                    if (att is IsFolderNameAttribute)
                    {
                        isFolderName = true;
                    }

                    if (att is ParameterFromListAttribute)
                    {
                        var listAtt = att as ParameterFromListAttribute;
                        isParameterList = true;
                        listPropName = listAtt.ListName;
                        listType = listAtt.Type;
                    }

                    if (att is ParameterNameAttribute)
                    {
                        var nameAtt = att as ParameterNameAttribute;
                        name = nameAtt.ParameterName;
                    }
                    if (att is ParameterRangeAttribute)
                    {
                        var rangeAtt = att as ParameterRangeAttribute;
                        min = rangeAtt.Min;
                        max = rangeAtt.Max;
                    }
                    if (att is ParameterAttribute)
                    {
                        isparameter = true;
                    }
                    if (att is ParameterTipAttribute)
                    {
                        var tipAtt = att as ParameterTipAttribute;
                        tipMessage = tipAtt.Message;
                    }

                    if (att is ParameterEnabledFromPropertyAttribute)
                    {
                        var enableAtt = att as ParameterEnabledFromPropertyAttribute;
                        bindingBooleanProperty = enableAtt.PropertyName;
                        var propType = paramType.GetProperty(bindingBooleanProperty,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (propType == null || propType.PropertyType != typeof(Boolean))
                            bindingBooleanProperty = string.Empty;
                        else if (enableAtt.NotOperation)
                        {
                            valueConverter = new OpositeBooleanConverter();
                        }
                    }

                    if (att is ReadOnlyAttribute)
                        isReadOnly = true;
                }

                if (isparameter)
                {
                    var propType = property.PropertyType;
                    var template = TextErrorTemplate.GetTemplate();
                    Type itemType = null;
                    FrameworkElement control = null;
                    var controlType = ParameterControlManager.Instance.GetControlType(propType);
                    if (controlType != null)
                    {
                        var prop = property.GetValue(param);
                        ParameterControlBase pcb = ParameterControlManager.Instance.GetControl(prop);
                        control = pcb;
                    }
                    else if (isParameterList)
                    {
                        ComboBox cmb = new ComboBox();

                        var binding = new Binding
                        {
                            Path = new PropertyPath(property.Name, new object[0]),
                            Source = param
                        };
                        cmb.SetBinding(ComboBox.SelectedItemProperty, binding);

                        binding = new Binding
                        {
                            Path = new PropertyPath(listPropName, new object[0]),
                            Source = param
                        };
                        cmb.SetBinding(ComboBox.ItemsSourceProperty, binding);

                        control = cmb;
                        cmb.IsReadOnly = isReadOnly;
                    }
                    else if (IsNumber(propType))
                    {
                        TextBox txt = new TextBox();
                        var binding = new Binding { Path = new PropertyPath(property.Name, new object[0]), Converter = GetToStringConverter(propType), Source = param };
                        var validationRule = new NumberRangeValidationRule { Min = min, Max = max };
                        binding.ValidationRules.Add(validationRule);
                        txt.SetBinding(TextBox.TextProperty, binding);
                        Validation.SetErrorTemplate(txt, template);
                        binding = new Binding
                        {
                            Path = new PropertyPath("Text", new object[0]),
                            Source = txt
                        };
                        txt.SetBinding(TextBox.ToolTipProperty, binding);
                        control = txt;
                        txt.IsReadOnly = isReadOnly;
                    }
                    else if (propType == typeof(string))
                    {
                        Grid grid = new Grid();

                        TextBox txt = new TextBox();
                        var binding = new Binding { Path = new PropertyPath(property.Name, new object[0]), Source = param };
                        txt.SetBinding(TextBox.TextProperty, binding);
                        binding = new Binding
                        {
                            Path = new PropertyPath("Text", new object[0]),
                            Source = txt
                        };
                        grid.Children.Add(txt);
                        txt.SetBinding(TextBox.ToolTipProperty, binding);
                        txt.IsReadOnly = isReadOnly;
                        if (isFileName || isFolderName)
                        {
                            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(90, GridUnitType.Star) });
                            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(25) });
                            var button = new Button();
                            button.Content = "...";
                            if (isFileName)
                            {
                                button.Click += new RoutedEventHandler(delegate (object o, RoutedEventArgs args)
                                   {
                                       var dlg = new Microsoft.Win32.OpenFileDialog();
                                       dlg.Multiselect = false;
                                       dlg.Filter = "Any files (*.*)|*.*";
                                       if (dlg.ShowDialog() ?? false)
                                       {
                                           property.SetValue(param, dlg.FileName);
                                       }
                                   }
                                   );
                            }
                            else
                            {
                                button.Click += new RoutedEventHandler(delegate (object o, RoutedEventArgs args)
                                {
                                    var dlg = new System.Windows.Forms.FolderBrowserDialog();
                                    if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                    {
                                        property.SetValue(param, dlg.SelectedPath);
                                    }
                                }
                                   );
                            }
                            grid.Children.Add(button);
                            Grid.SetColumn(button, 1);
                        }
                        control = grid;
                    }
                    else if (propType.IsEnum)
                    {
                        ComboBox cmb = new ComboBox();
                        foreach (var obj in Enum.GetValues(propType))
                        {
                            cmb.Items.Add(obj);
                        }

                        var d = new DataTemplate();

                        var textBlock = new FrameworkElementFactory(typeof(TextBlock));
                        var binding = new Binding { Converter = new EnumToStringConverter() };
                        textBlock.SetBinding(TextBlock.TextProperty, binding);
                        d.VisualTree = textBlock;
                        cmb.ItemTemplate = d;

                        binding = new Binding { Path = new PropertyPath(property.Name, new object[0]), Source = param };

                        cmb.SetBinding(ComboBox.SelectedItemProperty, binding);
                        cmb.IsReadOnly = isReadOnly;
                        control = cmb;
                    }
                    else if (propType == typeof(Boolean))
                    {
                        ComboBox cmb = new ComboBox();
                        cmb.Items.Add(true);
                        cmb.Items.Add(false);

                        var binding = new Binding { Path = new PropertyPath(property.Name, new object[0]), Source = param };
                        cmb.SetBinding(ComboBox.SelectedItemProperty, binding);
                        control = cmb;
                        cmb.IsReadOnly = isReadOnly;
                    }
                    else if (typeof(ObjectBase).IsAssignableFrom(propType))
                    {
                        ParameterListSmall pls = new ParameterListSmall();
                        var binding = new Binding
                        {
                            Path = new PropertyPath(property.Name, new object[0]),
                            Source = param
                        };
                        pls.SetBinding(ParameterListSmall.ItemProperty, binding);
                        control = pls;
                    }
                    else if (ClassUtilities.TypeIsList(propType, out itemType))
                    {
                        var pls = new ParameterListSmall { IsList = true };

                        var binding = new Binding
                        {
                            Path = new PropertyPath(property.Name, new object[0]),
                            Source = param
                        };
                        pls.SetBinding(ParameterListSmall.ObjectsProperty, binding);
                        control = pls;
                    }

                    if (control != null)
                    {
                        if (!string.IsNullOrEmpty(tipMessage))
                        {
                            control.ToolTip = tipMessage;
                        }

                        if (!string.IsNullOrEmpty(bindingBooleanProperty))
                        {
                            var binding = new Binding
                            {
                                Path = new PropertyPath(bindingBooleanProperty, new object[0]),
                                Source = param
                            };
                            if (valueConverter != null)
                                binding.Converter = valueConverter;
                            control.SetBinding(GetDependencyPropertyByName(control, "IsEnabledProperty"), binding);
                        }

                        itemsCache.Add(new Pair<string, FrameworkElement>(name, control));
                    }
                }
            }
            return itemsCache;
        }

        public static string GetParameterName(PropertyInfo property)
        {
            var attrs = property.GetCustomAttributes();
            if (attrs.Any(i => i is ParameterNameAttribute))
            {
                var attr = attrs.First(i => i is ParameterNameAttribute) as ParameterNameAttribute;

                return attr.ParameterName;
            }
            return property.Name;
        }

        public static string GetParameterTip(PropertyInfo property)
        {
            var attrs = property.GetCustomAttributes();
            if (attrs.Any(i => i is ParameterTipAttribute))
            {
                var attr = attrs.First(i => i is ParameterTipAttribute) as ParameterTipAttribute;

                return attr.Message;
            }
            return string.Empty;
        }

        public static bool IsNumber(Type propType)
        {
            return propType == typeof(int) || propType == typeof(float) || propType == typeof(double) || propType == typeof(long);
        }

        public static IValueConverter GetToStringConverter(Type type)
        {
            if (type == typeof(int))
            {
                return new IntToStringConverter();
            }

            if (type == typeof(long))
                return new LongToStringConverter();

            if (type == typeof(double))
                return new DoubleToStringConverter();

            return null;
        }

        public static DependencyProperty GetDependencyPropertyByName(DependencyObject dependencyObject, string dpName)
        {
            return GetDependencyPropertyByName(dependencyObject.GetType(), dpName);
        }

        public static DependencyProperty GetDependencyPropertyByName(Type dependencyObjectType, string dpName)
        {
            DependencyProperty dp = null;

            var fieldInfo = dependencyObjectType.GetField(dpName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (fieldInfo != null)
            {
                dp = fieldInfo.GetValue(null) as DependencyProperty;
            }

            return dp;
        }
    }
}
