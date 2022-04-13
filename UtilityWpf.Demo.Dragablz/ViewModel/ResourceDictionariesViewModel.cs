using System.Collections;
using System.Linq;
using UtilityWpf.Demo.Common.ViewModel;

namespace UtilityWpf.Demo.Dragablz.ViewModel
{
    public class ResourceDictionariesViewModel : Common.ViewModel.ResourceDictionariesViewModel
    {
        private readonly TickViewModel[] collection;

        public ResourceDictionariesViewModel()
        {
            var dictionaries = typeof(UtilityWpf.Demo.Common.ViewModel.ResourceDictionariesViewModel)
                 .Assembly
                 .SelectResourceDictionaries(a => a.Key.ToString().EndsWith("themes.baml", System.StringComparison.CurrentCultureIgnoreCase))
                 .Single()
                 .resourceDictionary
                 .MergedDictionaries;

            collection = ThemesViewModelFactory
                .CreateViewModels(dictionaries)
                .ToArray();

            ResourceDictionaryService service = new(dictionaries);

            foreach (var item in collection)
            {
                service.OnNext(item as TickViewModel);
            }
        }

        public override IEnumerable Collection => collection;
    }
}