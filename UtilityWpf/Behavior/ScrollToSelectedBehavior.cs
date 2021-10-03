using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Threading;

namespace UtilityWpf.Behavior
{
    public class ScrollToSelectedBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
        }

        private static void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
                _ = listBox.Dispatcher.BeginInvoke(() =>
                {
                    if (listBox.SelectedItem != null)
                        listBox.ScrollIntoView(listBox.SelectedItem);
                }, DispatcherPriority.ContextIdle);
        }
    }
}