using Autofac;
using Splat.Autofac;
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
        public App()
        {
            // type has an assembly for which it is desirable to AutoRegister
            // (a better way should be found)

            var assemblies = AssemblySingleton.Instance.Assemblies
                .Where(a => a.GetName().Name.Contains("Demo"))
                .Where(a => a.DefinedTypes.Any(a => a.IsAssignableTo(typeof(UserControl))))
                .ToArray();

            InitialiseContainerBuilder()
            .AutoRegister()
            .UseAutofacDependencyResolver();

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewsControl(assemblies)
            }.Show();
        }

        private static ContainerBuilder InitialiseContainerBuilder()
        {
            return new ContainerBuilder();
        }
    }
}