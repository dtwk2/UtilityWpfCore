using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace UtilityWpf.DemoApp.View
{
    /// <summary>
    /// Interaction logic for DatesListUserControl.xaml
    /// </summary>
    public partial class DatesListUserControl : UserControl
    {
        public DatesListUserControl()
        {
            InitializeComponent();
            (DatesList.DatesChangeCommand as ICommand).Execute(DateTime.Now);
        }
    }
}