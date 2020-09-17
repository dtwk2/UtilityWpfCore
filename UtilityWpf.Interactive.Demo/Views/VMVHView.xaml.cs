using Splat;
using System.Windows.Controls;
using UtilityWpf.Abstract;

namespace UtilityWpf.Interactive.Demo
{
    /// <summary>
    /// Interaction logic for VMVHView.xaml
    /// </summary>
    public partial class VMVHView : UserControl
    {
        public VMVHView()
        {
            InitializeComponent();

            Init();
        }

        async void Init()
        {
            InteractiveList2.Data = await (Locator.Current.GetService<IViewModelAssemblyModel>()).Collection;
        }
    }
}