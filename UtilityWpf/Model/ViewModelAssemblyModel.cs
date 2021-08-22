using System.Threading.Tasks;
using Autofac;
using Nito.AsyncEx;
using UtilityWpf.Abstract;

namespace UtilityWpf.Model
{
    public class ViewModelAssemblyModel : IViewModelAssemblyModel
    {
        private readonly AsyncLazy<TypeObject[]> typeObjects;


        public ViewModelAssemblyModel(ITypeModel typeModel, ITypeObjectsService typeObjectsService)
        {
            typeObjects = new AsyncLazy<TypeObject[]>(() => Task.Run(async () => typeObjectsService.SelectTypeObjects(await typeModel.Collection)));
        }

        public async Task Register(ContainerBuilder containerBuilder)
        {
            foreach (var type in await typeObjects.Task)
                containerBuilder.RegisterType(type.Type);
        }

        public Task<TypeObject[]> Collection => typeObjects.Task;

    }
}