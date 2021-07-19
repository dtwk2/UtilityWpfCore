using MaterialDesignThemes.Wpf;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using UtilityWpf.Controls;

namespace UtilityWpf.DemoApp
{
    /// <summary>
    /// Interaction logic for ItemsControlUserControl.xaml
    /// </summary>
    public partial class ItemsControlUserControl : UserControl
    {
        public static readonly ObservableCollection<PackIconKind> collection = new ObservableCollection<PackIconKind>();
        public readonly ReactiveCommand<ListBox, (ListBox lbox, int count)> deselectCommand = ReactiveCommand.Create<ListBox, (ListBox, int)>((l) => (l, l.Items.Count));

        public ItemsControlUserControl()
        {
            InitializeComponent();
            collection.Add(PackIconKind.Cast);
            ItemsControl1.ItemsSource = collection;
            ItemsControl1.DataContext = this;

            ItemsControl2.ItemsSource = collection;
            ItemsControl2.DataContext = this;

            deselectCommand.Where(a => a.count == 1).Buffer(2).Subscribe(a =>
             {
                 a.First().lbox.SelectedIndex = -1;
             });
        }

        public ReactiveCommand<ListBox, (ListBox lbox, int count)> DeselectCommand => deselectCommand;
    }

    public class AddControl1 : AddControl
    {
        private readonly PackIconKind[] kinds;
        private readonly Random random = new Random();

        public AddControl1()
        {
            kinds = Enum.GetValues(typeof(PackIconKind)).Cast<PackIconKind>().ToArray();
        }

        public override void ExecuteAdd(object parameter)
        {
            ItemsControlUserControl.collection.Add(kinds[random.Next(0, kinds.Length - 1)]);
        }
    }
}