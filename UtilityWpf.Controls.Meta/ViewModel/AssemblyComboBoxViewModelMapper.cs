using ReactiveUI;
using System;
using System.Reactive.Linq;
using Utility.WPF.Reactive;
using System.Reactive.Disposables;

namespace UtilityWpf.Controls.Meta.ViewModel
{
    internal class AssemblyComboBoxViewModelMapper
    {
        public static IDisposable Connect(AssemblyComboBox assemblyComboBox, AssemblyComboBoxViewModel comboBoxViewModel)
        {
            CompositeDisposable composite = new();

            assemblyComboBox
                .WhenAnyValue(a => a.DemoType)
                .Subscribe(comboBoxViewModel.demoTypeViewModel)
                .DisposeWith(composite);

            comboBoxViewModel
                .demoTypeViewModel
                .Subscribe(a => assemblyComboBox.ItemsSource = a)
                .DisposeWith(composite);

            assemblyComboBox
                .SelectSingleSelectionChanges()
                .CombineLatest(assemblyComboBox.LoadedChanges(), (a, b) => a)
                .DistinctUntilChanged()
                .Subscribe(comboBoxViewModel.selectedItemViewModel)
                .DisposeWith(composite);

            comboBoxViewModel
                .selectedItemViewModel
                .Subscribe(a => assemblyComboBox.SelectedItem = a)
                .DisposeWith(composite);

            assemblyComboBox
                .SelectOutputChanges()
                .Subscribe(comboBoxViewModel.checkedViewModel)
                .DisposeWith(composite);

            return composite;
        }
    }
}
