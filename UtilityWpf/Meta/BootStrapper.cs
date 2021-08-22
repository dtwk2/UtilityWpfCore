using System;
using Autofac;
using UtilityWpf.Abstract;
using UtilityWpf.Model;
using UtilityWpf.Service;

namespace UtilityWpf.Meta
{
    public class BootStrapper
    {

        public static void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TypeModel>().As<ITypeModel>().UsingConstructor(() => new TypeModel());
            containerBuilder.RegisterType<TypeObjectsService>().As<ITypeObjectsService>();
            containerBuilder.RegisterType<ViewModelAssemblyModel>().As<IViewModelAssemblyModel>();
        }

        public static void Register(ContainerBuilder containerBuilder, Type[] types)
        {
            containerBuilder.Register(c => new TypeModel(types)).As<ITypeModel>();
            containerBuilder.RegisterType<TypeObjectsService>().As<ITypeObjectsService>();
            containerBuilder.RegisterType<ViewModelAssemblyModel>().As<IViewModelAssemblyModel>();

        }
    }
}
