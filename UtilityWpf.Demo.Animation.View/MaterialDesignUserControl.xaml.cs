using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace UtilityWpf.Demo.Animation.View
{
    /// <summary>
    /// Interaction logic for MaterialDesignUserControl.xaml
    /// </summary>
    public partial class MaterialDesignUserControl : UserControl
    {
        private readonly Dictionary<string, PackIconKind> values;

        public MaterialDesignUserControl()
        {
            InitializeComponent();

            this.DataContext = this;
            values = Enum.GetValues(typeof(PackIconKind)).Cast<PackIconKind>().GroupBy(a => a.ToString()).ToDictionary(a => a.Key.ToLowerInvariant(), a => a.First());
            MainItemsControl.ItemsSource = values.Values.Take(20);
        }
    }

    public class PackIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PackIconKind kind)
                return new PackIcon
                {
                    Width = 70,
                    Height = 70,
                    Margin = new Thickness(4),
                    Kind = kind
                };
            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static PackIconConverter Instance { get; } = new PackIconConverter();
    }
}
