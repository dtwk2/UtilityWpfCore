using ReactiveUI;
using Splat;
using System;
using System.Collections;
using System.Linq;
using System.Windows;

namespace UtilityWpf.Interactive.Demo.Common
{
    public class PropertyDataTemplateSelector : System.Windows.Controls.DataTemplateSelector
    {
        public DataTemplate DefaultDataTemplate { get; set; }
        public DataTemplate EnumerableDataTemplate { get; set; }
        public DataTemplate ContentPresenterTemplate { get; set; }
        public DataTemplate DictionaryDataTemplate { get; set; }
        public DataTemplate IConvertibleTemplate { get; set; }
        public DataTemplate ViewModelHostViewTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
                return DefaultDataTemplate;

            var type = item.GetType();

            //DataTemplate myDataTemplate = ((System.Windows.Controls.ContentPresenter)container).ContentTemplate;

            var dataTemplateKey = new DataTemplateKey(type);
            //var x = Application.Current.Resources[dataTemplateKey];
            object dataTemplate = null;
            if (dataTemplateKey != null)
                dataTemplate = (container as FrameworkElement).TryFindResource(dataTemplateKey);
            
            if (dataTemplate != null)
                return ContentPresenterTemplate;

            var interfaces = type.GetInterfaces();
            if (interfaces.Contains(typeof(IConvertible)))
                return IConvertibleTemplate;
            else if (interfaces.SingleOrDefault(_ => _.Name == "IDictionary`2") != null || interfaces.Contains(typeof(IDictionary)))
                return DictionaryDataTemplate;
            else if (interfaces.Contains(typeof(IEnumerable)))
                return EnumerableDataTemplate;
            else if (Locator.Current.GetService<IViewLocator>()?.ResolveView(item) is {} view)
                return ViewModelHostViewTemplate;
            else
                return DefaultDataTemplate;
        }
    }
}