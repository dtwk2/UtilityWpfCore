﻿using Autofac;
using Splat.Autofac;
using System.Windows;
using Utility.Common;
using UtilityWpf.Meta;

namespace UtilityWpf.Demo.Controls
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var builder = new ContainerBuilder();
            var d = typeof(UtilityWpf.Demo.Common.ViewModel.Tick);
            builder.AutoRegister();
            builder.UseAutofacDependencyResolver();

            new Window
            {
                WindowState = WindowState.Maximized,
                Content = new UserControls()
            }.Show();
        }

    }
}
