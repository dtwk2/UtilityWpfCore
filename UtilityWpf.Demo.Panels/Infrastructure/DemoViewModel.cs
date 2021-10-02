using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityWpf.Demo.Panels
{
    public class DemoViewModel
    {
        public DemoViewModel()
        {

        }

        public ObservableCollection<string> Collection { get; } = new ObservableCollection<string>(Enumerable.Range(0, 10).Select(a => a.ToString()));
        public string[] HalfCollection { get; } = Enumerable.Range(0, 4).Select(a => a.ToString()).ToArray();

        public static DemoViewModel Instance { get; } = new DemoViewModel();
    }
}
