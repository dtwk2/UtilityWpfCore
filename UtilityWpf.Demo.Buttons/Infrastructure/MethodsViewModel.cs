using System.Linq;
using System.Collections.ObjectModel;
using ReactiveUI;
using UtilityHelperEx;

namespace UtilityWpf.Demo.Buttons
{
    public class MethodsViewModel
    {
        public MethodsViewModel()
        {
            Data = new ObservableCollection<ButtonViewModel>(
                UtilityHelper.ReflectionHelper.GetMethods(new Model())
                .Select(a => new ButtonViewModel(a.Item1, ReactiveCommand.Create(() => { _ = a.Item2(); }))));                
        }

        public ObservableCollection<ButtonViewModel> Data { get; }

    }

}