using Autofac;
using Splat.Autofac;
using System.Windows;
using Utility.Common;
using UtilityWpf.Meta;

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
            builder.RegisterInstance(
                new AutoMapperTypeCollection( 
                typeof(UtilityWpf.Demo.Common.Infrastructure.Profile)));
            builder.UseAutofacDependencyResolver();
        }
    }
}