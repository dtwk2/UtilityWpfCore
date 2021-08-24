using System.Collections;
using System.Windows;
using Evan.Wpf;

namespace UtilityWpf.Controls
{
    public class MasterBindableControl : MasterControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyHelper.Register<IEnumerable>();
        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyHelper.Register<string>();

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
    }
}
