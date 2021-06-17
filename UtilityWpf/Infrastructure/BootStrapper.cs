using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using UtilityWpf.Abstract;
using UtilityWpf.Model;
using UtilityWpf.Service;

namespace UtilityWpf.Infrastructure
{
    public class BootStrapper
    {

        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TypeModel>().As<ITypeModel>();
            containerBuilder.RegisterType<TypeObjectsService>().As<ITypeObjectsService>();
            containerBuilder.RegisterType<ViewModelAssemblyModel>().As<IViewModelAssemblyModel>();

        }
    }
}
