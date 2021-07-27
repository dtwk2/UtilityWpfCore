//using ReactiveUI;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reactive;
//using System.Reflection;
//using System.Windows.Input;
//using UtilityInterface.NonGeneric;
//using UtilityWpf.Attribute;

//namespace UtilityWpf.DemoApp.ViewModel
//{
//    public class ViewModelsViewModel : ReactiveObject
//    {
//        private readonly ReactiveCommand<Unit, Unit> forceGcCommand;
//        private Lazy<KeyValuePair<string, KeyValuePair<string, object>>[]> types;
//        private object selectedItem;

//        public ViewModelsViewModel(string viewModelAssembly = "UtilityWpf.ViewModel")
//        {
//            types = new Lazy<KeyValuePair<string, KeyValuePair<string, object>>[]>
//                (() =>
//                {
//                    Exception exception = null ;
//                    try
//                    {
//                        return ViewModelFinder.Select(viewModelAssembly);
//                    }
//                    catch (Exception ex)
//                    {
//                       // var see = typeof(UtilityWpf.ViewModel.NavigatorViewModel).Assembly.FullName;
//                        exception = ex;
//                    }
//                    return new[] { KeyValuePair.Create("Error", KeyValuePair.Create(exception.Message, default(object))) };
//                });

//            forceGcCommand = ReactiveCommand.Create(
//                () =>
//                {
//                    GC.Collect();
//                    GC.WaitForPendingFinalizers();
//                    GC.Collect();
//                });
//        }

//        public IEnumerable Collection => types.Value;

//        public ICommand ForceGCCommand => forceGcCommand;

//        public object SelectedItem { get => selectedItem; set => this.RaiseAndSetIfChanged(ref selectedItem, value); }

//        class ViewModelFinder
//        {
//            public static KeyValuePair<string, KeyValuePair<string, object>>[] Select(string viewModelAssembly)
//            {
//                var tt = Assembly.Load(viewModelAssembly).GetTypes();

//                var ss = tt
//                       .Where(type => type.GetCustomAttribute<ViewModelAttribute>() != null)
//                       .ToArray();

//                var xs = ss
//                       .SelectMany(type => Splat.Locator.Current.GetServices(type).Select(a => (a, type))).ToArray();

//                return xs.Select(st =>
//                {
//                    var (service, type) = st;
//                    var name = typeof(IName).IsAssignableFrom(type) ?
//                                                    (service as IName).Name :
//                                                     type.Name;
//                    return new KeyValuePair<string, KeyValuePair<string, object>>(
//                        type.Name,
//                        new KeyValuePair<string, object>(name, service));
//                }).ToArray();
//            }
//        }
//    }
//}
