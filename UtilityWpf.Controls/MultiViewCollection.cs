using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls
{
    public class MultiCollectionView : ItemsControl
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(MultiCollectionView), new PropertyMetadata(Orientation.Vertical));

        static MultiCollectionView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiCollectionView), new FrameworkPropertyMetadata(typeof(MultiCollectionView)));
        }

        public MultiCollectionView()
        {
        }

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ItemsControl;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var button = new ItemsControl();
            return button;
        }
    }
}