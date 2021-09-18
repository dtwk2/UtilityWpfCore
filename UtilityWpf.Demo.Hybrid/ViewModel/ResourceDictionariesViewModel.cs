using Utility.Persist;
using System.Collections;
using UtilityWpf.Demo.Common.ViewModel;
using UtilityWpf.Service;
using UtilityInterface.NonGeneric.Database;

namespace UtilityWpf.Demo.Hybrid.ViewModel
{
    public class ResourceDictionariesViewModel : Common.ViewModel.ResourceDictionariesViewModel
    {
        private readonly CollectionService collectionService = new();

        public ResourceDictionariesViewModel()
        {
            ResourceDictionaryService service = new();
            collectionService.OnNext(new(DatabaseService()));
            foreach (var item in new TickViewModelFactory().Collection)
            {
                if (collectionService.Items.Contains(item) == false)
                    collectionService.Items.Add(item);
                //else
                //    collectionService.Items.Remove(item);
            }

            foreach(var item in collectionService.Items)
            {
                service.OnNext(item as TickViewModel);
            }
        }


        public override IEnumerable Collection => collectionService.Items;

        IDatabaseService DatabaseService() => new LiteDbRepository(
            new LiteDbRepository.ConnectionSettings(typeof(TickViewModel),
                new System.IO.FileInfo("../../../Data/Data.litedb"),
                nameof(TickViewModel.Id)));

    }
}
