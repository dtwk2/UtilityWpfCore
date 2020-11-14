using Autofac;
using ReactiveUI;
using Splat;
using Splat.Autofac;
using System.Windows;
using UtilityWpf.Interactive.Demo;
using UtilityWpf.Interactive.Demo.ViewModel;

namespace UtilityWpf.Interactive
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var containerBuilder = new ContainerBuilder();

            Infrastructure.BootStrapper.Register(containerBuilder);

            containerBuilder.RegisterType<ViewModelAssemblyViewModel>();

            containerBuilder.UseAutofacDependencyResolver();

            Locator.CurrentMutable.Register(()=> new TestViewModel());
            Locator.CurrentMutable.Register<IViewFor<ViewModelAssemblyViewModel>>(() => new ViewModelAssemblyView());
            Locator.CurrentMutable.Register<IViewFor<TestViewModel>>(() => new TestView());
            Locator.CurrentMutable.Register<IViewFor<Test1ViewModel>>(() => new Test2View());
            Locator.CurrentMutable.Register<IViewFor<Test2ViewModel>>(() => new Test3View());
            Locator.CurrentMutable.Register<IViewFor<Test3ViewModel>>(() => new Test4View());
        }
    }
}