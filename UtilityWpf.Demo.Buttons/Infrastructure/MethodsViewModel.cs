using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using UtilityHelperEx;
using UtilityWpf.Demo.Buttons.Infrastructure;

namespace UtilityWpf.Demo.Buttons
{
    public class MethodsViewModel
    {
        public MethodsViewModel()
        {
            Data = new ObservableCollection<ButtonViewModel>(
                UtilityHelper.ReflectionHelper.GetMethods(new UtilityWpf.Demo.Buttons.Infrastructure.Model())
                .Select(a => new ButtonViewModel(a.Item1, ReactiveCommand.Create(() => { _ = a.Item2(); }))));
        }

        public ObservableCollection<ButtonViewModel> Data { get; }
    }
}