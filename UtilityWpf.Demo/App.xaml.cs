using Autofac;
using Splat.Autofac;
using System.Reflection;
using System.Windows;
using Utility.Common;

namespace UtilityWpf.DemoApp
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