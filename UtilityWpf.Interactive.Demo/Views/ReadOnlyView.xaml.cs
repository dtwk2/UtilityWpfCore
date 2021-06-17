using System.Windows.Controls;

namespace UtilityWpf.Interactive.Demo
{
    /// <summary>
    /// Interaction logic for ListBoxExUserControl.xaml
    /// </summary>
    public partial class ReadOnlyView : UserControl
    {
        public ReadOnlyView()
        {
            InitializeComponent();

            Group_ToggleButton.Checked += ToggleGroup_Checked;
            Group_ToggleButton.Unchecked += ToggleGroup_Checked;

            Orientation_ToggleButton.Checked += Orientation_ToggleButton_Checked;
            Orientation_ToggleButton.Unchecked += Orientation_ToggleButton_Checked;

            GroupProperty_ToggleButton.Checked += GroupProperty_ToggleButton_Checked;
            GroupProperty_ToggleButton.Unchecked += GroupProperty_ToggleButton_Checked;
        }

        private void GroupProperty_ToggleButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Main_InteractiveList.Group = GroupProperty_ToggleButton.IsChecked.Value ? "Last" : "First";
        }

        private void Orientation_ToggleButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Main_InteractiveList.Orientation = Orientation_ToggleButton.IsChecked.Value ? Orientation.Vertical : Orientation.Horizontal;
        }

        private void ToggleGroup_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            Main_InteractiveList.Group = Group_ToggleButton.IsChecked.Value ? "Last" : null;
        }
    }
}