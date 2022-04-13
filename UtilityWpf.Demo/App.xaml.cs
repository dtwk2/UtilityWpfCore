using Autofac;
using Splat.Autofac;
using System.Windows;
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
            new ContainerBuilder()
            .AutoRegister()
            .UseAutofacDependencyResolver();

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new AssemblyViewsControl()
            }.Show();
        }

    }
}