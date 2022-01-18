using Autofac;
using Splat.Autofac;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Utility.Common;
using UtilityWpf.Controls.Meta;

namespace UtilityWpf.Demo
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        const string DemoAppNameAppendage = "Demo";
        public App()
        {
            new ContainerBuilder()
            .AutoRegister()
            .UseAutofacDependencyResolver();

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewsControl(FindDemoAppAssemblies().ToArray())
            }.Show();
        }

        private static IEnumerable<System.Reflection.Assembly> FindDemoAppAssemblies()
        {
            return from a in AssemblySingleton.Instance.Assemblies
                   where a.GetName().Name.Contains(DemoAppNameAppendage)
                   where a.DefinedTypes.Any(a => a.IsAssignableTo(typeof(UserControl)))
                   select a;
        }
    }
}