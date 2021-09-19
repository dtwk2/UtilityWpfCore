using Autofac;
using Splat.Autofac;
using System.Windows;

namespace UtilityWpf.Demo.Master
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var builder = new ContainerBuilder();
            builder.UseAutofacDependencyResolver();
        }
    }
}