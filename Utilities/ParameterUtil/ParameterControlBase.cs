using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CoilSimulater.Utilities
{
    public class ParameterControlBase : UserControl
    {
        public static DependencyProperty ParameterProperty = DependencyProperty.Register("Parameter", typeof(object),
            typeof(ParameterControlBase), new FrameworkPropertyMetadata(null));

        public object Parameter
        {
            get { return GetValue(ParameterProperty); }
            set { SetValue(ParameterProperty, value); }
        }

        public ParameterControlBase() { }

        public ParameterControlBase(object param)
        {
            Parameter = param;
        }
    }
}
