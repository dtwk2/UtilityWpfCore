using System.Windows;

namespace UtilityWpf.Demo.Forms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //(this.DataContext as MainViewModel)
            //    .Changes()
            //    .Subscribe(a =>
            //    {
            //        BindingExpression be = JsonControl.GetBindingExpression(UtilityWpf.Controls.Objects.JsonControl.ObjectProperty);
            //        be.UpdateSource();
            //    });
        }
    }
}