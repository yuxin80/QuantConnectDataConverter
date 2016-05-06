using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CoilSimulater.Utilities
{
    public class ParameterCustomControlBase : UserControl
    {
        public static DependencyProperty ObjectsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(ParameterCustomControlBase), new PropertyMetadata(null));

        public IEnumerable Items
        {
            get { return GetValue(ObjectsProperty) as IEnumerable; }
            set { SetValue(ObjectsProperty, value); }
        }

        public static DependencyProperty IsListProperty = DependencyProperty.Register("IsList", typeof(Boolean), typeof(ParameterCustomControlBase), new PropertyMetadata(false));

        public Boolean IsList
        {
            get { return (Boolean)GetValue(IsListProperty); }
            set { SetValue(IsListProperty, value); }
        }

        public static DependencyProperty ItemProperty = DependencyProperty.Register("Item", typeof(ObjectBase), typeof(ParameterCustomControlBase), new PropertyMetadata(null));

        public ObjectBase Item
        {
            get { return GetValue(ItemProperty) as ObjectBase; }
            set { SetValue(ItemProperty, value); }
        }
    }
}
