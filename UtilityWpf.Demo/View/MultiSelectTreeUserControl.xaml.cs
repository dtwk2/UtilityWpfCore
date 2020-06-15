using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using UtilityInterface.Generic;
using UtilityWpf.Abstract;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for MultiSelectTreeView.xaml
    /// </summary>
    public partial class MultiSelectTreeView : UserControl
    {
        public MultiSelectTreeView()
        {
            InitializeComponent();
            this.DataContext = new TreeViewModel();
        }
    }

    public class TreeViewModel
    {
        public TreeViewModel()
        {
            RootNodes = BuildTreeModel();
        }

        private static IList<TreeItemModel> BuildTreeModel()
        {
            return new[]
            {
                new TreeItemModel("", 1),
                         new TreeItemModel("",2)
            };
            //    {
            //        //IsExpanded = true,
            //        Children = new List<TreeItemViewModel>
            //        {
            //            new TreeItemViewModel("Node 1.1"),
            //            new TreeItemViewModel("Node 1.2")
            //            {
            //                Children = new List<TreeItemViewModel>
            //                {
            //                    new TreeItemViewModel("Node 1.2.1"),
            //                    new TreeItemViewModel("Node 1.2.2"),
            //                    new TreeItemViewModel("Node 1.2.3"),
            //                    new TreeItemViewModel("Node 1.2.4"),
            //                    new TreeItemViewModel("Node 1.2.5"),
            //                    new TreeItemViewModel("Node 1.2.6")
            //                }
            //            }
            //        }
            //    },
            //    new TreeItemViewModel("Node 2")
            //    {
            //        Children = new List<TreeItemViewModel>
            //        {
            //            new TreeItemViewModel("Node 2.2.1"),
            //            new TreeItemViewModel("Node 2.2.2"),
            //            new TreeItemViewModel("Node 2.2.3"),
            //            new TreeItemViewModel("Node 2.2.4")
            //        }
            //    }
            //};
        }

        public IList<TreeItemModel> RootNodes { get; }
    }

    public class TreeItemModel : IParent<TreeItemModel>, IDelayedConstructor
    {
        private string _number;

        public TreeItemModel(string parent, int number)
        {
            _number = parent + number;
            Name = "Node " + _number;
        }

        //public bool IsExpanded {
        //    get;
        //    set; }

        //public bool IsSelected
        //{
        //    get;
        //    set;
        //}

        public string Name { get; }

        public IEnumerable<TreeItemModel> Children { get; set; }

        public Task<bool> Init(object o) =>
            Task.Run(() =>
            {
                Children = new[]
                 {
                new TreeItemModel(_number, 1),
                         new TreeItemModel(_number,2)
            }; return true;
            });
    }
}