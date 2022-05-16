using System.Windows;
using System.Windows.Controls;
using Utility.WPF.Helper;

namespace UtilityWpf.DataTemplateSelectors
{
    public class CustomDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item != null)
            {
                var dataTemplateKey = new DataTemplateKey(item.GetType());
                var dataTemplate = ((FrameworkElement)container).TryFindResource(dataTemplateKey);
                if (dataTemplate != null)
                    return (DataTemplate)dataTemplate;
                else
                {
                    return TemplateGenerator.CreateDataTemplate(() => new TextBlock {
                        FontSize = 14,
                        HorizontalAlignment= HorizontalAlignment.Stretch,
                        VerticalAlignment= VerticalAlignment.Stretch,
                        Text = $"Missing DataTemplate for type, {item.GetType().Name}" 
                    });
                }
            }
            else
            {
                return TemplateGenerator.CreateDataTemplate(() => new TextBlock {
                    FontSize = 14,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Text = $"item is null" });
            }
        }

        public static CustomDataTemplateSelector Instance => new CustomDataTemplateSelector();
    }
}