using DynamicData;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using UtilityWpf.Abstract;
using UtilityWpf.Model;

namespace UtilityWpf.Interactive
{
    public class ViewModelAssemblyViewModel : ReactiveObject
    {
        private readonly ReadOnlyObservableCollection<TypeObject> collection;
        public ViewModelAssemblyViewModel(IViewModelAssemblyModel model)
        {

            model
                .Collection
                .ToObservable()
                .SelectMany(a => a)
                .ToObservableChangeSet()
                .Bind(out collection)
                .Subscribe();
        }

        public ReadOnlyObservableCollection<TypeObject> Collection => collection;
    }
}
