using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Interop;

namespace CoilSimulater.Utilities
{
    public static class WpfHelper
    {
        public static IList<DependencyProperty> GetAttachedProperties(DependencyObject obj)
        {
            List<DependencyProperty> result = new List<DependencyProperty>();

            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(obj,
                new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.All) }))
            {
                DependencyPropertyDescriptor dpd =
                    DependencyPropertyDescriptor.FromProperty(pd);

                if (dpd != null)
                {
                    result.Add(dpd.DependencyProperty);
                }
            }

            return result;
        }

        public static Window GetParentWindow(FrameworkElement control)
        {
            var parent = control.Parent as FrameworkElement;
            if (parent is Window)
                return parent as Window;
            if (parent == null)
                return null;

            return GetParentWindow(parent);
        }

        public static Window ShowControlInNewWindow(FrameworkElement control, string title = "New window", int width = 640, int height = 480, bool fixSize = false)
        {
            var window = new Window();
            window.Title = title;
            window.Width = width;
            window.Height = height;
            window.MinHeight = height;
            window.MinWidth = width;
            window.Content = control;
            window.MinHeight = height;
            window.MinWidth = width;
            if (fixSize)
            {
                window.MaxHeight = height;
                window.MaxWidth = width;
            }
            var mainWindow = Process.GetCurrentProcess().MainWindowHandle;
            var helper = new WindowInteropHelper(window);
            helper.Owner = mainWindow;
            //window.Owner = Application.Current.MainWindow;

            window.ShowInTaskbar = false;
            window.Show();
            ElementHost.EnableModelessKeyboardInterop(window);
            return window;
        }

        public static void ShowControlInNewDialog(FrameworkElement control, string title = "New window", int width = 640, int height = 480, bool fixSize = false)
        {
            var window = new Window();
            window.Title = title;
            window.Width = width;
            window.Height = height;
            window.Content = control;
            window.MinHeight = height;
            window.MinWidth = width;
            if (fixSize)
            {
                window.MaxHeight = height;
                window.MaxWidth = width;
            }

            var mainWindow = Process.GetCurrentProcess().MainWindowHandle;
            var helper = new WindowInteropHelper(window);
            helper.Owner = mainWindow;
            //window.Owner = Application.Current.MainWindow;
            window.ShowInTaskbar = false;
            ElementHost.EnableModelessKeyboardInterop(window);
            window.ShowDialog();
        }

        public static void ShowInMainForm(this Window window)
        {
            var mainWindow = Process.GetCurrentProcess().MainWindowHandle;
            //window.Owner = Application.Current.MainWindow;
            var helper = new WindowInteropHelper(window);
            helper.Owner = mainWindow;
            window.ShowInTaskbar = false;
            window.Show();
            ElementHost.EnableModelessKeyboardInterop(window);
        }

        public static bool? ShowDialogInMainForm(this Window window)
        {
            var mainWindow = Process.GetCurrentProcess().MainWindowHandle;
            //window.Owner = Application.Current.MainWindow;
            //var helper = new WindowInteropHelper(window);
            //helper.Owner = mainWindow;
            window.ShowInTaskbar = true;
            ElementHost.EnableModelessKeyboardInterop(window);
            return window.ShowDialog();
        }
    }
}
