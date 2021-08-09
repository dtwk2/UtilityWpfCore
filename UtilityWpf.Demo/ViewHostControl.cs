using MoreLinq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UtilityWpf.Controls;


namespace UtilityWpf.Demo.Infrastructure
{
    using static UtilityWpf.DependencyPropertyFactory<ViewHostControl>;

    class ViewType
    {
        public ViewType(string key, Type type)
        {
            Key = key;
            Type = type;
        }

        public string Key { get; }
        public Type Type { get; }
        public FrameworkElement View => (FrameworkElement)Activator.CreateInstance(Type);
    }
    public class ViewHostControl : MasterDetail
    {
        Subject<Assembly> subject = new();
        public static readonly DependencyProperty AssemblyProperty = Register(a => a.subject, initialValue: Assembly.GetEntryAssembly());

        public ViewHostControl()
        {
            UseDataContext = true;
            _ = subject
                .WhereNotNull()
              .Select(assembly =>
              {
                  var ucs = assembly
                   .GetTypes()
                   .Where(a => typeof(UserControl).IsAssignableFrom(a))
                   .GroupBy(type =>
                   (type.Name.Contains("UserControl") ? type.Name?.Replace("UserControl", string.Empty) :
                   type.Name.Contains("View") ? type.Name?.Replace("View", string.Empty) :
                   type.Name)!)
                   .OrderBy(a => a.Key)
                   .ToDictionaryOnIndex()
                   .Select(a => new ViewType(a.Key, a.Value));
                  return ucs.ToArray();
              })
              .Subscribe(pairs => ItemsSource = pairs);

            this.Content = CreateContent();
        }

        static Grid CreateContent()
        {
                //        < materialDesign:TransitioningContent
                // Width = "200"
                //Opacity = "0"
                //OpeningEffectsOffset = "{materialDesign:IndexedItemOffsetMultiplierExtension 0:0:0.1}"
                //RunHint = "Loaded" >
                //< materialDesign:TransitioningContent.OpeningEffects >
 
                //     < materialDesign:TransitionEffect Kind = "SlideInFromLeft" />
  
                //      < materialDesign:TransitionEffect Kind = "FadeIn" />
   
                //   </ materialDesign:TransitioningContent.OpeningEffects >

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Auto) });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1.0, GridUnitType.Star) });
            var textBlock = new TextBlock
            {
                Margin = new Thickness(20),
                FontSize = 20
            };
            grid.Children.Add(textBlock);
            Binding binding = new()
            {
                Path = new PropertyPath(nameof(ViewType.Key)),
            };
            textBlock.SetBinding(TextBlock.TextProperty, binding);
            var contentControl = new ContentControl { Content="Empty" };
            Grid.SetRow(contentControl, 1);
            binding = new Binding
            {
                Path = new PropertyPath(nameof(ViewType.View)),
        
            };
            contentControl.SetBinding(ContentControl.ContentProperty, binding);
            grid.Children.Add(contentControl);
            return grid;
        }
   

        public Assembly Assembly
        {
            get { return (Assembly)GetValue(AssemblyProperty); }
            set { SetValue(AssemblyProperty, value); }
        }
    }

    public static class Helper
    {
        public static Dictionary<string, T> ToDictionaryOnIndex<T>(this IEnumerable<IGrouping<string, T>> groupings)
            => groupings
           .SelectMany(grp => grp.Index().ToDictionary(kvp => kvp.Key > 0 ? grp.Key + kvp.Key : grp.Key, c => c.Value))
          .ToDictionary(a => a.Key, a => a.Value);
    }
}


