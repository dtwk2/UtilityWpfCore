using ReactiveUI;
using Splat;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UtilityWpf.Interactive.Demo.ViewModel;
using UtilityWpf.Interactive.Demo;

namespace UtilityWpf.Interactive
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public App()
        {
            Locator.CurrentMutable.Register<IViewFor<TestViewModel>>(() => new TestView());
            Locator.CurrentMutable.Register<IViewFor<Test1ViewModel>>(() => new Test2View());
            Locator.CurrentMutable.Register<IViewFor<Test2ViewModel>>(() => new Test3View());
            Locator.CurrentMutable.Register<IViewFor<Test3ViewModel>>(() => new Test4View());
        }
    }
}
