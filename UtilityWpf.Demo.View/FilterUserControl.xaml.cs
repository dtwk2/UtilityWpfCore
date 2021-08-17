using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComputerCastleControls;

namespace UtilityWpf.Demo.View
{
    /// <summary>
    /// Interaction logic for FilterUserControl.xaml
    /// </summary>
    public partial class FilterUserControl : UserControl
    {
        private readonly ICollectionView view;

        public FilterUserControl()
        {
            InitializeComponent();
            var characters = this.TryFindResource("Characters") as IEnumerable;
            view = CollectionViewSource.GetDefaultView(characters);
            listBox.ItemsSource = view;
        }



        private void TypeControl_Changed_2(object sender, Controls.TypeControl.ChangedEventArgs e)
        {
            view.Filter = item =>
            {
                if (e.Value == null)
                {
                    return true;
                }
                PropertyInfo info = item.GetType().GetProperty(e.Property);
                if (info == null)
                    return false;

                return info.GetValue(item, null).ToString().ToLower().Contains(e.Value.ToLower());
            };
        }
    }
}
