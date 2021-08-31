using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Xml;

namespace UtilityWpf.Utility
{
    /// <summary>
    /// Extension methods for the WPF Binding class.
    /// </summary>
    public static class BindingHelper
    {
        /// <summary>
        /// Clone via xaml-Serialization
        /// <a href="https://stackoverflow.com/questions/32541/how-can-you-clone-a-wpf-object"></a>
        /// </summary>
        public static BindingBase Clone(this BindingBase binding)
        {
            StringBuilder sb = new();
            Save(binding, sb);
            return Load(sb.ToString());

            static BindingBase Load(string value)
            {
                return XamlReader.Load(XmlReader.Create(new StringReader(value))) switch
                {
                    null => throw new ArgumentNullException("Binding could not be cloned via Xaml Serialization Stack."),
                    Binding newBinding => newBinding,
                    MultiBinding newBinding => newBinding,
                    PriorityBinding newBinding => newBinding,
                    _ => throw new InvalidOperationException("Binding could not be cast.")
                };
            }

            static void Save(BindingBase binding, StringBuilder sb)
            {
                var mgr = new XamlDesignerSerializationManager(XmlWriter.Create(sb, new XmlWriterSettings
                {
                    Indent = true,
                    ConformanceLevel = ConformanceLevel.Fragment,
                    OmitXmlDeclaration = true,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                }));

                // HERE BE MAGIC!!!
                mgr.XamlWriterMode = XamlWriterMode.Expression;
                // THERE WERE MAGIC!!!

                XamlWriter.Save(binding, mgr);
            }
        }

        /// <summary>
        /// <a href="https://stackoverflow.com/questions/32541/how-can-you-clone-a-wpf-object"></a>
        /// </summary>
        public static T Clone<T>(this T element) where T : UIElement
        {
            string xaml = XamlWriter.Save(element);
            StringReader stringReader = new StringReader(xaml);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            T clone = (T)XamlReader.Load(xmlReader);
            return clone;
        }
    }
}
