using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace UtilityWpf.View
{
    //<TemplatePart(Name:="PART_IndicatorList", Type:=typeof(ItemsControl))> _
    public class ListBoxWithSelectedItemIndicator : System.Windows.Controls.ContentControl
    {
        #region private Declarations

        private ItemsControl _objIndicatorList;
        private System.Collections.ObjectModel.ObservableCollection<double> _objIndicatorOffsets;
        private ListBox _objListBox;

        #endregion private Declarations

        #region public Declarations

        public static readonly DependencyProperty IndicatorBrushProperty = DependencyProperty.Register("IndicatorBrush", typeof(Brush), typeof(ListBoxWithSelectedItemIndicator), new PropertyMetadata(new LinearGradientBrush(Colors.LightBlue, Colors.Blue, new Point(0.5, 0), new Point(0.5, 1))));
        public static readonly DependencyProperty IndicatorHeightWidthProperty = DependencyProperty.Register("IndicatorHeightWidth", typeof(Double), typeof(ListBoxWithSelectedItemIndicator), new PropertyMetadata(16.0));

        #endregion public Declarations

        #region Properties

        //< Description("Brush used to paint the indicator.  Defaults to a nice blue gradient brush"), Category("Custom")> _
        public Brush IndicatorBrush
        {
            get { return (Brush)GetValue(IndicatorBrushProperty); }
            set { SetValue(IndicatorBrushProperty, value); }
        }

        public double IndicatorHeightWidth
        {
            get { return (double)GetValue(IndicatorHeightWidthProperty); }
            set { SetValue(IndicatorHeightWidthProperty, value); }
        }

        #endregion Properties

        #region " Methods "

        static ListBoxWithSelectedItemIndicator()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ListBoxWithSelectedItemIndicator), new FrameworkPropertyMetadata(typeof(ListBoxWithSelectedItemIndicator)));
        }

        public ListBoxWithSelectedItemIndicator()
        {
            Uri resourceLocater = new Uri("/UtilityWpf.View;component/Themes/ListBoxEx.xaml", System.UriKind.Relative);
            ResourceDictionary resourceDictionary = (ResourceDictionary)Application.LoadComponent(resourceLocater);
            Style = resourceDictionary["ListBoxWithSelectedItemIndicator"] as Style;

            this.Loaded += ListBoxWithSelectedItemIndicator_Loaded;
            this.Unloaded += ListBoxWithSelectedItemIndicator_Unloaded;
        }

        //public override void OnContentChanged(Object oldContent, Object newContent)
        //     {
        //         //this is our insurance policy that the developer does not add content that is not a ListBox

        //         if (newContent is null || newContent is ListBox)
        //         {
        //             //this ensures that our reference to the child ListBox is always corect or null.
        //             //if(the child ListBox is removed, our reference is set to null
        //             //if(the child ListBox is swapped out, our reference is set to the newContent
        //             _objListBox = newContent as ListBox;

        //             //this removes our references to the ListBox items
        //             if (!(_objIndicatorOffsets is null) && _objIndicatorOffsets.Count > 0)
        //                 _objIndicatorOffsets.Clear();

        //         }

        //         else
        //             throw new System.NotSupportedException("Invalid content.  ListBoxWithSelectedItemIndicator only accepts a ListBox control as its content.");
        //     }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //when the template is applied, this give the developer the oppurtunity to get references to name objects in the control template.
            //in our case, we need a reference to the ItemsControl that holds the indicator arrows.
            //
            //what your control does in the absence of an expected object in the control template is up to the control develeper.
            //in my case here, without the items control, we are dead in the water.
            //
            //remember that custom controls are supposed to be Lookless.  Meaning the visual and code are highly decoupled.
            //Any designer using Blend fully expects to be able edit the control template anyway they want.
            //My using the "PART_" naming convention, you indicate that this object is probably necessary for the conrol to work, but this is not true in all cases.
            //
            _objIndicatorList = GetTemplateChild("PART_IndicatorList") as ItemsControl;
            if (_objIndicatorList is null)
                throw new Exception("The PART_IndicatorList is missing from the template or is not an ItemsControl. ItemsControl is required.");
        }

        private void ListBoxWithSelectedItemIndicator_Loaded(Object sender, System.Windows.RoutedEventArgs e)
        {
            if (_objIndicatorList is null)
                //remember how much "fun" tabs were be in VB and Access?  Well...
                //
                //this is here because when you place a custom control in a tab, it loads the control once before it runs OnApplyTemplate
                //when the TabItem its in gets clicked (focus), OnApplyTemplate runs ) Loaded runs agin.
                //
                //since OnApplyTemplate has not run yet, we are out of here

                _objIndicatorOffsets = new System.Collections.ObjectModel.ObservableCollection<double>();
            _objIndicatorList.ItemsSource = _objIndicatorOffsets;

            //How cool are routed events!  We can listen into the child ListBoxes activities and act accordingly.
            AddHandler(ListBox.SelectionChangedEvent, new SelectionChangedEventHandler(ListBox_SelectionChanged));
            AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(ListBox_ScrollViewer_ScrollChanged));

            UpdateIndicators();
        }

        private void UpdateIndicators()
        {
            //This is the awesome procedure that Josh Smith authored with a few modifications

            if (_objIndicatorOffsets is null)
                return;

            if (_objListBox is null)
                return;

            if (!(_objIndicatorOffsets is null) && _objIndicatorOffsets.Count > 0)
                _objIndicatorOffsets.Clear();

            if (_objListBox.SelectedItems.Count == 0)
                return;

            var objGen = _objListBox.ItemContainerGenerator;
            if (objGen.Status != System.Windows.Controls.Primitives.GeneratorStatus.ContainersGenerated)
                return;

            foreach (var objSelectedItem in _objListBox.SelectedItems)
            {
                var lbi = (objGen.ContainerFromItem(objSelectedItem) as ListBoxItem);
                if (lbi is null)
                    continue;

                var objTransform = lbi.TransformToAncestor(_objListBox);
                var pt = objTransform.Transform(new Point(0, 0));

                var dblOffset = pt.Y + (lbi.ActualHeight / 2) - (IndicatorHeightWidth / 2);
                _objIndicatorOffsets.Add(dblOffset);
            }
        }

        private void ListBox_ScrollViewer_ScrollChanged(Object sender, System.Windows.Controls.ScrollChangedEventArgs e)
        {
            //if(the user is scrolling horizontality, no reason to run any of our attached behavior code
            if (e.VerticalChange == 0)
                return;

            UpdateIndicators();
        }

        private void ListBox_SelectionChanged(Object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateIndicators();
        }

        private void ListBoxWithSelectedItemIndicator_Unloaded(Object sender, System.Windows.RoutedEventArgs e)
        {
            RemoveHandler(ListBox.SelectionChangedEvent, new SelectionChangedEventHandler(ListBox_SelectionChanged));
            RemoveHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(ListBox_ScrollViewer_ScrollChanged));
        }

        #endregion " Methods "
    }
}