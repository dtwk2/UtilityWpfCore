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
using UtilityWpf.Interactive.Demo.Views;

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
        }
    }
}
