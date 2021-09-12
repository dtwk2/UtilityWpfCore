using Autofac;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Utility.Common;
using UtilityWpf.Model;
using UtilityWpf.Service;

namespace UtilityWpf.Meta
{


    //public class BootStrapperTypeCollection : Collection<Type>
    //{
    //    public BootStrapperTypeCollection(IReadOnlyCollection<Type> types)
    //    {
    //        this.Add(typeof(BootStrapperTypeCollection));
    //        foreach (var type in types)
    //            this.Add(type);
    //    }

    //    public BootStrapperTypeCollection(params Type[] types) : this(types as IReadOnlyCollection<Type>)
    //    {
    //    }
    //}

    public class BootStrapper : IBootStrapper
    {

        public void Register(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TypeModel>();
            containerBuilder.RegisterType<TypeObjectsService>();
            containerBuilder.RegisterType<ViewModelAssemblyModel>();
        }

        //public static void Register(ContainerBuilder containerBuilder, Type[] types)
        //{
        //    containerBuilder.Register(c => new TypeModel(types)).As<ITypeModel>();
        //    containerBuilder.RegisterType<TypeObjectsService>().As<ITypeObjectsService>();
        //    containerBuilder.RegisterType<ViewModelAssemblyModel>().As<IViewModelAssemblyModel>();

        //}
    }
}
