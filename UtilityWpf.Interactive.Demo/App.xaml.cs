using Autofac;
using ReactiveUI;
using Splat;
using Splat.Autofac;
using System.Windows;
using UtilityWpf.Abstract;
using UtilityWpf.Interactive.Demo;
using UtilityWpf.Interactive.Demo.ViewModel;

namespace UtilityWpf.Interactive
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IViewModelAssemblyModel model;

        public App()
        {
            var containerBuilder = new ContainerBuilder();

            Infrastructure.BootStrapper.Register(containerBuilder);

            containerBuilder.RegisterType<ViewModelAssemblyViewModel>();

            containerBuilder.Register((a)=> new TestViewModel());
            containerBuilder.Register<IViewFor<ViewModelAssemblyViewModel>>((a) => new ViewModelAssemblyView());
            containerBuilder.Register<IViewFor<TestViewModel>>((a) => new TestView());
            containerBuilder.Register<IViewFor<Test1ViewModel>>((a) => new Test2View());
            containerBuilder.Register<IViewFor<Test2ViewModel>>((a) => new Test3View());
            containerBuilder.Register<IViewFor<Test3ViewModel>>((a) => new Test4View());

            containerBuilder.UseAutofacDependencyResolver();

            var build = containerBuilder.Build();

            model = build.Resolve<IViewModelAssemblyModel>();

        }

        public IViewModelAssemblyModel Model => model;
    }
}