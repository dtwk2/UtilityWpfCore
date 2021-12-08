using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UtilityWpf.Demo.Common.Meta;
using UtilityWpf.Demo.Common.ViewModel;

namespace UtilityWpf.Demo.Master.Infrastructure
{
    /// <summary>
    /// Interaction logic for PersistListUserControl.xaml
    /// </summary>
    public partial class MasterDetailGridUserControl : UserControl
    {
        public MasterDetailGridUserControl()
        {
            InitializeComponent();
            //(this.DataContext as PersistListViewModel).Data = PersistBehavior.Items;
        }

        private void DragablzItemsControl_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }


    public class CollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Statics.Service<Factory>().Create<Fields>(Statics.Random.Next(10, 20));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static CollectionConverter Instance { get; } = new CollectionConverter();
    }
}
