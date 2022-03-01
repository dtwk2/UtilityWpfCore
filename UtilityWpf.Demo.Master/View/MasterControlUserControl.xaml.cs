using System.Collections;
using System.Linq;
using System.Windows.Controls;
using UtilityWpf.Demo.Data.Factory;

namespace UtilityWpf.Demo.Master.View
{
    /// <summary>
    /// Interaction logic for ItemsWrapUserControl.xaml
    /// </summary>
    public partial class MasterControlUserControl : UserControl
    {
        public MasterControlUserControl()
        {
            InitializeComponent();
        }
    }

    public class MasterControlViewModel
    {
        private static FieldsFactory Factory { get; } = new();

        public IEnumerable Data { get; } = Factory.BuildCollection().ToArray();
    }
}