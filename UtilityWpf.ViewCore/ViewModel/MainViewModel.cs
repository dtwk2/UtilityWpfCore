using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reactive;
using System.Reflection;
using System.Windows.Input;
using UtilityInterface.NonGeneric;

namespace UtilityWpf.View
{
    public class MainViewModel : ReactiveObject
    {

        private const string ViewModelAssembly = "ViewModelAssembly";
        private readonly ReactiveCommand<Unit, Unit> forceGcCommand;
        private readonly Lazy<KeyValuePair<string, KeyValuePair<string, object>>[]> types;
        private object selectedItem;
     
        public MainViewModel()
        {
            types = Utility.LazyEx.Create(Select);

            this.forceGcCommand = ReactiveCommand.Create(
                () =>
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                });
        }

        public IEnumerable Collection => this.types.Value;

        public ICommand ForceGCCommand => this.forceGcCommand;

        public object SelectedItem { get => selectedItem; set => this.RaiseAndSetIfChanged(ref selectedItem, value); }

        private static KeyValuePair<string, KeyValuePair<string, object>>[] Select()
        {
            var tt = Assembly.Load(ConfigurationManager.AppSettings[ViewModelAssembly]).GetTypes();

            //Splat.Locator.Current.GetServices(App.)
            return tt
                   .Where(type => type.GetCustomAttribute<Attribute.ViewModelAttribute>() != null ||
                                  type.Name.EndsWith("ViewModel"))
                   .SelectMany(type =>
                         Splat.Locator.Current.GetServices(type)
                                                 .Select(service =>
                                                 {
                                                     var name = typeof(IName).IsAssignableFrom(type) ?
                                                                                     (service as IName).Name :
                                                                                      type.Name;
                                                     return new KeyValuePair<string, KeyValuePair<string, object>>(
                                                         type.Name,
                                                         new KeyValuePair<string, object>(name, service));
                                                 })).ToArray();
        }

    }
}
