using System.Collections;
using System.Windows;
using Evan.Wpf;
using ReactiveUI;

namespace UtilityWpf.Controls
{
    public class MasterBindableControl : MasterControl
    {
        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyHelper.Register<string>();

        static MasterBindableControl()
        {

        }

        public MasterBindableControl()
        {
        }

        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
    }
}
