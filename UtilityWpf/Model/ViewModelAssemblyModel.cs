using Autofac;
using MoreLinq;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UtilityInterface.NonGeneric;
using UtilityWpf.Abstract;

namespace UtilityWpf.Model
{
    public class ViewModelAssemblyModel : IViewModelAssemblyModel
    {
        private readonly Lazy<Type[]> types;
        private readonly AsyncLazy<TypeObject[]> typeObjects;


        public ViewModelAssemblyModel(ITypeModel typeModel, ITypeObjectsService typeObjectsService)
        {
            typeObjects = new AsyncLazy<TypeObject[]>(() => Task.Run(async () => typeObjectsService.SelectTypeObjects(await typeModel.Collection)));
        }

        public void Register(ContainerBuilder containerBuilder)
        {
            foreach (var type in types.Value)
                containerBuilder.RegisterType(type);
        }

        public Task<TypeObject[]> Collection => typeObjects.Task;

    }
}