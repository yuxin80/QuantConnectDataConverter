using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CoilSimulater.Utilities.Templates
{

    public static class TextErrorTemplate
    {
        private static readonly string c_TemplateXaml = "<ControlTemplate x:Key=\"TextBoxErrorTemplate\">\n            <Grid>\n                <!-- Placeholder for the TextBox itself -->\n                <Border BorderBrush=\"Red\" BorderThickness=\"1\"></Border>\n                <AdornedElementPlaceholder x:Name=\"textBox\">\n                </AdornedElementPlaceholder>\n                <Image Width=\"15\" Height=\"15\" Stretch=\"Fill\" Margin=\"0,0,3,0\" HorizontalAlignment=\"Right\" VerticalAlignment=\"Center\">\n                    <Image.Source>\n                        <ImageSource>Images/error.ico</ImageSource>\n                    </Image.Source>\n                    <Image.ToolTip>\n                        <ToolTip Content=\"{Binding [0].ErrorContent}\" Foreground=\"Red\"/>\n                    </Image.ToolTip>\n                </Image>\n            </Grid>\n        </ControlTemplate>";

        private static ControlTemplate m_TemplateCache;

        public static ControlTemplate GetTemplate()
        {
            if (m_TemplateCache == null)
            {
                //m_TemplateCache = new ControlTemplate();

                //m_TemplateCache.VisualTree = new System.Windows.FrameworkElementFactory(typeof(TextErrorTemplateVisualtree));
                MemoryStream sr = new MemoryStream(Encoding.ASCII.GetBytes(c_TemplateXaml));
                ParserContext pc = new ParserContext();
                pc.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                pc.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                m_TemplateCache = (ControlTemplate)XamlReader.Load(sr, pc);
            }
            return m_TemplateCache;
        }
    }
}
