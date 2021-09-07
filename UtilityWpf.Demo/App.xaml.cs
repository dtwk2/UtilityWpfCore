using Autofac;
using Splat.Autofac;
using System.Windows;
using UtilityWpf.Meta;

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
            _ = builder.RegisterInstance(
                new AutoMapperTypeCollection(
                typeof(UtilityWpf.Demo.Common.Infrastructure.Profile)));
            builder.UseAutofacDependencyResolver();
        }
    }
}