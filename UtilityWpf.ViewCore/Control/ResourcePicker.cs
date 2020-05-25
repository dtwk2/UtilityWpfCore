using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace UtilityWpf.View
{
    public class ResourcePicker : Control
    {
        private ComboBox ComboBox;

        public override void OnApplyTemplate()
        {
            ComboBox = this.GetTemplateChild("ComboBox") as ComboBox;
            ComboBox.SelectionChanged += ComboBox_SelectionChanged;
            Change();
        }

        static ResourcePicker()
        {
             DefaultStyleKeyProperty.OverrideMetadata(typeof(ResourcePicker), new FrameworkPropertyMetadata(typeof(ResourcePicker)));
        }

        public ResourcePicker()
        {
        }

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set { SetValue(PathProperty, value); }
        }

        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(ResourcePicker), new PropertyMetadata(null, PathChanged));

        private static void PathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ResourcePicker).Change();
        }

        private void Change()
        {
            if (ComboBox != null)
                this.Dispatcher.InvokeAsync(() =>
            ComboBox.ItemsSource = System.IO.Directory.GetFiles(Path).Select(a => System.IO.Path.GetFileNameWithoutExtension(a)), System.Windows.Threading.DispatcherPriority.Background);
        }

        string[] keys = null;

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string fileName = System.IO.Path.Combine(Path, ComboBox.SelectedItem.ToString() + ".xaml");
            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    // Read in ResourceDictionary File
                    ResourceDictionary dic = (ResourceDictionary)XamlReader.Load(fs);
                    // Clear any previous dictionaries loaded
                    if (keys != null)
                    {
                        var remove = Application.Current.Resources.MergedDictionaries.Select((a, i) => (a, i)).SingleOrDefault(c => c.a.Keys.OfType<string>().SequenceEqual(keys)).i;
                        Application.Current.Resources.MergedDictionaries.RemoveAt(remove);
                    }
                    // Add in newly loaded Resource Dictionary
                    Application.Current.Resources.MergedDictionaries.Add(dic);
                    keys = dic.Keys.OfType<string>().ToArray();
                }
            }
        }
    }
}