using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls
{
    /// <summary>
    /// Interaction logic for TableUserControl.xaml
    /// </summary>
    public partial class TableUserControl : UserControl
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(IEnumerable), typeof(TableUserControl));

        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public TableUserControl()
        {
            InitializeComponent();
            usercontrol.DataContext = this;
        }
    }
}