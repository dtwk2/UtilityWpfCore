using Autofac;
using Splat.Autofac;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Utility.Common;
using UtilityWpf.Meta;

namespace UtilityWpf.Demo.Hybrid
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
