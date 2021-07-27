﻿# nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;

namespace UtilityWpf.Controls
{
    /// <summary>
    /// Interaction logic for ObjectView.xaml
    ///<a href=http://www.codeproject.com/Tips/469452/WPF-ObjectView">Adapted from </a>
    /// </summary>
    public partial class ObjectView : UserControl
    {
        /// <summary>
        /// Gets the value of the AssemblyProduct attribute of the app.
        /// If unable to lookup the attribute, returns an empty string.
        /// </summary>
        public static string Product => _product ?? (_product = AssemblyHelper.GetProductName());

        private static string? _defaultTitle = null;
        private static string? _product = null;

        // Font sizes based on the "normal" size.
        //double _small;
        //double _med;
        //double _large;

        // This is used to dynamically calculate the mainGrid.MaxWidth when the Window is resized,
        // since I can't quite get the behavior I want without it.  See CalcMaxTreeWidth().
        private double _chromeWidth;

        private bool _initialized;

        public static readonly DependencyProperty ControlsBorderBrushProperty = DependencyProperty.Register(nameof(ControlsBorderBrush), typeof(Brush), typeof(ObjectView), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty ObjectProperty = DependencyProperty.Register(nameof(Object), typeof(object), typeof(ObjectView), new PropertyMetadata(default(Exception), OnObjectChanged));

        public static readonly RoutedEvent ObjectChangedEvent = EventManager.RegisterRoutedEvent(nameof(ObjectChanged), RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<Exception>), typeof(ObjectView));

        public static readonly DependencyProperty ShowDetailsProperty = DependencyProperty.Register(nameof(ShowDetails), typeof(bool), typeof(ObjectView), new PropertyMetadata(true, OnShowDetailPropertyChanged));

        public string InnerProperty
        {
            get { return (string)GetValue(InnerPropertyProperty); }
            set { SetValue(InnerPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InnerProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InnerPropertyProperty =
            DependencyProperty.Register("InnerProperty", typeof(string), typeof(ObjectView), new PropertyMetadata(null));

        public ObjectView() : this(null, null)
        {
        }

        /// <summary>
        /// The exception and header message cannot be null.  If owner is specified, this window
        /// uses its Style and will appear centered on the Owner.  You can override this before
        /// calling ShowDialog().
        /// </summary>
        public ObjectView(string? headerMessage, object? e)
        {
            InitializeComponent();
            Initialize(headerMessage, e);

            void Initialize(string? headerMessage, object? e)
            {
                if (!_initialized)
                {
                    Loaded += (sender, args) =>
                    {
                        treeCol.Width = new GridLength(treeCol.ActualWidth, GridUnitType.Pixel);
                        _chromeWidth = ActualWidth - mainGrid.ActualWidth;
                        ToggleDetails();
                        CalcMaxTreeWidth();
                    };

                    if (DefaultPaneBrush != null)
                    {
                        treeView1.Background = DefaultPaneBrush;
                    }

                    docViewer.Background = treeView1.Background;
                }

                _initialized = true;
                docViewer.Document = null;

                if (e != null)
                    this.Dispatcher.Invoke(() =>
                    {
                        BuildTree(treeView1, e, headerMessage, InnerProperty);
                    });
            }
        }

        public static Brush? DefaultPaneBrush { get; set; }

        /// <summary>
        /// The default title to use for the ObjectView window.  Automatically initialized
        /// to "Error - [ProductName]" where [ProductName] is taken from the application's
        /// AssemblyProduct attribute (set in the AssemblyInfo.cs file).  You can change this
        /// default, or ignore it and set Title yourself before calling ShowDialog().
        /// </summary>
        public static string DefaultTitle
        {
            get => _defaultTitle ??= "Error" + (string.IsNullOrEmpty(Product) ? string.Empty : $" - {Product}");
            set => _defaultTitle = value;
        }

        public object Object
        {
            get => (object)GetValue(ObjectProperty);
            set => SetValue(ObjectProperty, value);
        }

        public event RoutedPropertyChangedEventHandler<Exception> ObjectChanged
        {
            add => AddHandler(ObjectChangedEvent, value);
            remove => RemoveHandler(ObjectChangedEvent, value);
        }

        public Brush ControlsBorderBrush
        {
            get => (Brush)GetValue(ControlsBorderBrushProperty);
            set => SetValue(ControlsBorderBrushProperty, value);
        }

        public bool ShowDetails
        {
            get => (bool)GetValue(ShowDetailsProperty);
            set => SetValue(ShowDetailsProperty, value);
        }

        private static void OnObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = (ObjectView)d;
            var args = new RoutedPropertyChangedEventArgs<Exception>(
                (Exception)e.OldValue,
                (Exception)e.NewValue)
            {
                RoutedEvent = ObjectView.ObjectChangedEvent
            };
            instance.RaiseEvent(args);
            instance.docViewer.Document = null;
            instance.Dispatcher.Invoke(() =>
            {
                BuildTree(instance.treeView1, args.NewValue, innerProperty: instance.InnerProperty);
            });
        }

        private static void OnShowDetailPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ObjectView ObjectView)
            {
                ObjectView.ToggleDetails();
            }
        }

        private void ToggleDetails()
        {
            if (ShowDetails)
            {
                treeView1.Visibility = Visibility.Visible;
                gridSplitter.Visibility = Visibility.Visible;
                Grid.SetColumn(docViewer, 2);
                Grid.SetColumnSpan(docViewer, 1);
            }
            else
            {
                treeView1.Visibility = Visibility.Collapsed;
                gridSplitter.Visibility = Visibility.Collapsed;
                Grid.SetColumn(docViewer, 0);
                Grid.SetColumnSpan(docViewer, 3);
            }
        }

        private static dynamic GetFontSizes(TreeView treeView)
        {
            dynamic runTimeObject = new ExpandoObject();
            runTimeObject.Small = treeView.FontSize;
            runTimeObject.Medium = treeView.FontSize * 1.1;
            runTimeObject.Large = treeView.FontSize * 1.2;
            return runTimeObject;
        }

        private void TreeView1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ShowCurrentItem();
        }

        private void ChkWrap_Checked(object sender, RoutedEventArgs e)
        {
            ShowCurrentItem();
        }

        private void ChkWrap_Unchecked(object sender, RoutedEventArgs e)
        {
            ShowCurrentItem();
        }

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            // Build a FlowDocument with Inlines from all top-level tree items.

            Copy(treeView1);
            // The Inlines that were being displayed are now in the temporary document we just built,
            // causing them to disappear from the viewer.  This puts them back.

            ShowCurrentItem();
        }

        private object lck = new object();

        private void ShowCurrentItem()
        {
            if (treeView1.SelectedItem != null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    var doc = GetFlowDocument();

                    //if (chkWrap.IsChecked == false)
                    //{
                    //    doc.PageWidth = CalcNoWrapWidth(inlines) + 50;
                    //}

                    var para = new Paragraph();

                    if ((treeView1.SelectedItem as TreeViewItem)?.Tag is IEnumerable<Inline> inlines)
                        foreach (var line in inlines)
                        {
                            para.Inlines.Add(line);
                        }

                    doc.Blocks.Add(para);

                    docViewer.Document = doc;
                });
            }
        }

        private FlowDocument GetFlowDocument() => new FlowDocument
        {
            FontSize = GetFontSizes(treeView1).Small,
            FontFamily = treeView1.FontFamily,
            TextAlignment = TextAlignment.Left,
            Background = docViewer.Background
        };

        // Determines the page width for the Inlilness that causes no wrapping.
        private static double CalcNoWrapWidth(IEnumerable<Inline> inlines)
        {
            double pageWidth = 0;
            var tb = new TextBlock();
            var size = new Size(double.PositiveInfinity, double.PositiveInfinity);

            foreach (var inline in inlines)
            {
                tb.Inlines.Clear();
                tb.Inlines.Add(inline);
                tb.Measure(size);

                if (tb.DesiredSize.Width > pageWidth) pageWidth = tb.DesiredSize.Width;
            }

            return pageWidth;
        }

        // Builds the tree in the left pane.
        // Each TreeViewItem.Tag will contain a list of Inlines
        // to display in the right-hand pane When it is selected.
        private static async void BuildTree(TreeView treeView, object? obj, string? summaryMessage = null, string? innerProperty = null)
        {
            var fontSizes = GetFontSizes(treeView);
            var inlines = new List<Inline>();

            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            treeView.Items.Clear();

            if (!string.IsNullOrEmpty(summaryMessage))
            {
                var firstItem = new TreeViewItem { Header = "Summary" };
                treeView.Items.Add(firstItem);
                var inline = new Bold { FontSize = fontSizes.Large };

                ReplaceWithLinks(inline, summaryMessage);

                inlines.Add(inline);
                inlines.Add(new LineBreak());
                firstItem.Tag = inlines;
                firstItem.IsSelected = true;
                inlines.Add(new LineBreak());
            }

            // Now add top-level nodes for each exception while building
            // the contents of the first node.
            while (obj != null)
            {
                var aa = await GetObjectInformation(obj, innerProperty, fontSizes);
                var items = (aa.Properties as IEnumerable)
                                .Cast<(string name, Inline[] inlines)>()
                                .ToArray();

                treeView.Items.Add(new TreeViewItem
                {
                    IsSelected = treeView.Items.Count == 0,
                    IsExpanded = treeView.Items.Count == 0,
                    Header = aa.Name,
                    Tag = inlines.Concat(items.SelectMany(ac => ac.inlines)).ToArray(),
                    ItemsSource = items.Select(va => new TreeViewItem { Header = va.name, Tag = va.inlines }).ToArray()
                });

                obj = await Task.Run(() => GetInnerObject(obj, innerProperty));
            }

            static object? GetInnerObject(object obj, string? innerProperty)
            {
                return !(string.IsNullOrEmpty(innerProperty))
                     ? obj.GetType().GetProperty(innerProperty).GetValue(obj)
                     : null;
            }
        }

        // Adds the exception as a new top-level node to the tree with child nodes
        // for all the exception's properties.
        private static async Task<dynamic> GetObjectInformation(object e, string? innerProperty, dynamic fontSizes)
        {
            // Create a list of Inlines containing all the properties of the exception object.
            var type = await Task.Run(() => e.GetType());
            var inlines = new Inline[]
            {
                new Bold(new Run(type.ToString()))    {   FontSize = fontSizes.Large },
                new LineBreak()
            }.ToList();

            return new
            {
                type.Name,
                inlines,
                Properties = EnumerateProperties(await Task.Run(() => GetInformation(e).ToArray()), innerProperty, fontSizes.Medium).ToArray()
            };

            static IEnumerable<(PropertyInfo, object?)> GetInformation(object e) => e.GetType().GetProperties().Select(info => (info, info.GetValue(e, null)));

            static IEnumerable<(string name, Inline[] inlines)> EnumerateProperties((PropertyInfo, object?)[] props, string? innerProperty, double fontSize)
            {
                foreach (var (info, value) in props)
                {
                    // Skip InnerProperty because it will get a whole
                    // top-level node of its own.
                    if (innerProperty != null && info.Name == innerProperty)
                        continue;

                    if (value == null ||
                        (value is string s && string.IsNullOrEmpty(s)) ||
                        (value is IDictionary data && string.IsNullOrEmpty(RenderDictionary(data))) ||
                        (value is IEnumerable enumerable && !(enumerable is string) && string.IsNullOrEmpty(RenderEnumerable(enumerable))))
                        continue;

                    yield return (info.Name, EnumerateLines(info.Name, value, fontSize).ToArray());
                }

                static string RenderEnumerable(IEnumerable data)
                {
                    var result = new StringBuilder();

                    foreach (var obj in data)
                    {
                        result.AppendFormat("{0}\n", obj);
                    }

                    if (result.Length > 0) result.Length -= 1;
                    return result.ToString();
                }

                static string RenderDictionary(IDictionary data)
                {
                    var result = new StringBuilder();

                    foreach (var key in data.Keys)
                    {
                        if (key != null && data[key] != null)
                        {
                            result.AppendLine(key + " = " + data[key]);
                        }
                    }

                    if (result.Length > 0) result.Length -= 1;
                    return result.ToString();
                }
            }

            static IEnumerable<Inline> EnumerateLines(string propName, object propVal, double fontSize)
            {
                yield return new LineBreak();
                yield return new Bold(new Run(propName + ":")) { FontSize = fontSize };

                yield return new LineBreak();

                if (propVal is string str)
                {
                    // Might have embedded newlines.
                    foreach (var line in EnumerateInnerLines(str))
                        yield return line;
                }
                else
                {
                    yield return new Run(propVal.ToString());
                }
                yield return new LineBreak();

                // Adds the string to the list of Inlines, substituting
                // LineBreaks for an newline chars found.
                static IEnumerable<Inline> EnumerateInnerLines(string str)
                {
                    var lines = str.Split('\n');

                    yield return new Run(lines[0].Trim('\r'));

                    foreach (var line in lines.Skip(1))
                    {
                        yield return new LineBreak();
                        yield return new Run(line.Trim('\r'));
                    }
                }
            }
        }

        private static void Copy(TreeView treeView)
        {
            var inlines = new List<Inline>();
            var doc = new FlowDocument();
            var para = new Paragraph();

            doc.FontSize = GetFontSizes(treeView).Small;
            doc.FontFamily = treeView.FontFamily;
            doc.TextAlignment = TextAlignment.Left;

            foreach (TreeViewItem treeItem in treeView.Items)
            {
                if (inlines.Any())
                {
                    // Put a line of underscores between each exception.

                    inlines.Add(new LineBreak());
                    inlines.Add(new Run("____________________________________________________"));
                    inlines.Add(new LineBreak());
                }

                if (treeItem.Tag != null && treeItem.Tag is IEnumerable<Inline> tagInlines)
                {
                    inlines.AddRange(tagInlines);
                }
            }

            para.Inlines.AddRange(inlines);
            doc.Blocks.Add(para);

            // Now place the doc contents on the clipboard in both
            // rich text and plain text format.

            var range = new TextRange(doc.ContentStart, doc.ContentEnd);
            var data = new DataObject();

            using (Stream stream = new MemoryStream())
            {
                range.Save(stream, DataFormats.Rtf);
                data.SetData(DataFormats.Rtf, Encoding.UTF8.GetString((stream as MemoryStream).ToArray()));
            }

            data.SetData(DataFormats.StringFormat, range.Text);
            Clipboard.SetDataObject(data);
        }

        private static void ReplaceWithLinks(Span inline, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            var filePathRegex = new Regex(@"((?:[a-zA-Z]\:){0,1}(?:[\\/][\w.]+){1,})", RegexOptions.Compiled);
            var filePathsMatches = filePathRegex.Matches(message);

            if (filePathsMatches.Count > 0)
            {
                var parts = filePathRegex.Split(message);
                foreach (var part in parts)
                {
                    if (filePathRegex.IsMatch(part) && Uri.TryCreate(part, UriKind.Absolute, out var uri))
                    {
                        var hyperLink = new Hyperlink
                        {
                            NavigateUri = uri,
                            ToolTip = $"Open: {part}",
                            Inlines =
                            {
                                part
                            }
                        };
                        hyperLink.RequestNavigate += HyperLinkOnRequestNavigate;
                        inline.Inlines.Add(hyperLink);
                    }
                    else
                    {
                        inline.Inlines.Add(new Run(part));
                    }
                }
            }
            else
            {
                inline.Inlines.Add(message);
            }

            static void HyperLinkOnRequestNavigate(object sender, RequestNavigateEventArgs e)
            {
                if (e.Uri != null)
                {
                    Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                }
                e.Handled = true;
            }
        }

        private void CalcMaxTreeWidth()
        {
            // This prevents the GridSplitter from being dragged beyond the right edge of the window.
            // Another way would be to use star sizing for all Grid columns including the left
            // Grid column (i.e. treeCol), but that causes the width of that column to change when the
            // window's width changes, which I don't like.

            // mainGrid.MaxWidth = ActualWidth - _chromeWidth;
            treeCol.MaxWidth = mainGrid.MaxWidth - textCol.MinWidth;
        }

        private static class AssemblyHelper
        {
            // Initializes the Product property.
            public static string GetProductName()
            {
                var result = "";

                try
                {
                    var customAttributes = GetAppAssembly()?.GetCustomAttributes(typeof(AssemblyProductAttribute), false);

                    if (customAttributes != null && customAttributes.Length > 0)
                    {
                        result = ((AssemblyProductAttribute)customAttributes[0]).Product;
                    }
                }
                catch
                {
                    // ignored
                }

                return result;

                // Tries to get the assembly to extract the product name from.
                static Assembly GetAppAssembly()
                {
                    Assembly appAssembly = null;

                    try
                    {
                        // This is supposedly how Windows.Forms.Application does it.
                        appAssembly = Application.Current.MainWindow.GetType().Assembly;
                    }
                    catch
                    {
                        // ignored
                    }

                    // If the above didn't work, try less desireable ways to get an assembly.

                    if (appAssembly == null)
                    {
                        appAssembly = Assembly.GetEntryAssembly();
                    }

                    return appAssembly ?? (appAssembly = Assembly.GetExecutingAssembly());
                }
            }
        }
    }
}