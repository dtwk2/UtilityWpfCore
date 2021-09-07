using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UtilityWpf.Meta
{
    public interface IBootStrapper
    {
       // static abstract void Register(ContainerBuilder containerBuilder);
    }


    public class BootStrapperTypeCollection : Collection<Type>
    {
        public BootStrapperTypeCollection(IReadOnlyCollection<Type> types)
        {
            this.Add(typeof(BootStrapperTypeCollection));
            foreach (var type in types)
                this.Add(type);
        }

        public BootStrapperTypeCollection(params Type[] types) : this(types as IReadOnlyCollection<Type>)
        {
        }
    }

    public class BootStrapper
    {

        //public static void Register(ContainerBuilder containerBuilder)
        //{
        //    containerBuilder.RegisterType<TypeModel>().As<ITypeModel>().UsingConstructor(() => new TypeModel());
        //    containerBuilder.RegisterType<TypeObjectsService>().As<ITypeObjectsService>();
        //    containerBuilder.RegisterType<ViewModelAssemblyModel>().As<IViewModelAssemblyModel>();
        //}

        //public static void Register(ContainerBuilder containerBuilder, Type[] types)
        //{
        //    containerBuilder.Register(c => new TypeModel(types)).As<ITypeModel>();
        //    containerBuilder.RegisterType<TypeObjectsService>().As<ITypeObjectsService>();
        //    containerBuilder.RegisterType<ViewModelAssemblyModel>().As<IViewModelAssemblyModel>();

        //}
    }
}
