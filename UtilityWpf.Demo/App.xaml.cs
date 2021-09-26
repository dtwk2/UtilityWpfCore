using Autofac;
using Splat.Autofac;
using System.Reflection;
using System.Windows;
using Utility.Common;
using UtilityWpf.Controls.Master;
using UtilityWpf.Demo.FileSystem;
using UtilityWpf.Demo.Hybrid;
using UtilityWpf.Demo.View.Animation;
using UtilityWpf.Demo.View.Panels;
using UtilityWpf.Demo.View;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {         
            var builder = new ContainerBuilder();
            builder.AutoRegister();
            builder.UseAutofacDependencyResolver();

            var a = typeof(BarUserControl);
            var b = typeof(CornerPanelView);
            var c = typeof(AdornerUserControl);
            var d = typeof(FileBrowserView);
            var e = typeof(MasterListUserControl);
            var f = typeof(MeasurementsUserControl);
            var g = typeof(UtilityWpf.Demo.Dragablz.NotesUserControl);

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new ViewsExDetailControl(new[] { c, a, b, d, e, f, g })
            }.Show();
        }
    }
}