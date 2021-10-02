using System;
using System.Windows.Controls;

namespace UtilityWpf.Demo.Dialogs
{
    /// <summary>
    /// Interaction logic for ExceptionDialogUserControl.xaml
    /// </summary>
    public partial class ExceptionDialogUserControl : UserControl
    {
        public ExceptionDialogUserControl()
        {
            InitializeComponent();

            ExceptionDialog.Exception = new NullReferenceException("yret erewrwe eewew");
        }
    }
}
