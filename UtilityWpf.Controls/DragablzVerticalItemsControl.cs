using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Dragablz;
using ReactiveUI;
using UtilityWpf.Abstract;

namespace UtilityWpf.Controls
{
    public class DragablzVerticalItemsControl : DragablzItemsControl, ISelectionChanged
    {
        object number;
        double start = 0;
        private DeleteAdorner? adorner;
        private AdornerLayer layer;

        public static readonly RoutedEvent SelectionChangedEvent = EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(DragablzVerticalItemsControl));



        public DragablzVerticalItemsControl()
        {
            this.MouseDown += DragablzVerticalItemsControl_MouseDown;
            var VerticalPositionMonitor = new VerticalPositionMonitor();
            VerticalPositionMonitor.OrderChanged += VerticalPositionMonitor_OrderChanged;
            VerticalPositionMonitor.LocationChanged += VerticalPositionMonitor_LocationChanged;
            this.PositionMonitor = VerticalPositionMonitor;
            var customOrganiser = new CustomOrganiser();
            customOrganiser.DragCompleted += CustomOrganiser_DragCompleted;
            this.ItemsOrganiser = customOrganiser;
        }

        public event SelectionChangedEventHandler SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        private void CustomOrganiser_DragCompleted()
        {
            if (adorner != null)
            {
                //MessageBox.Show("Item deleted");
            }
        }

        private void VerticalPositionMonitor_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine(e.Location.Y);
            //if (number != e.Item)
            //{
            //    number = e.Item;
            //    start = e.Location.Y;
            //}
            //if (Math.Abs(e.Location.Y - start) > 50)
            //{
            //    if (adorner == null)
            //    {
            //        layer ??= AdornerLayer.GetAdornerLayer(this);
            //        adorner = new DeleteAdorner(this);
            //        layer.Add(adorner);
            //    }
            //    return;
            //}
            //adorner = null;
            //layer ??= AdornerLayer.GetAdornerLayer(this);
            //Adorner[] toRemoveArray = layer.GetAdorners(this);
            //if (toRemoveArray != null)
            //    foreach (Adorner a in toRemoveArray)
            //    {
            //        layer.Remove(a);
            //    }
        }

        private void VerticalPositionMonitor_OrderChanged(object? sender, OrderChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void DragablzVerticalItemsControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // throw new NotImplementedException();

        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            var item = base.GetContainerForItemOverride();
            (item as DragablzItem)?
                .WhenAny(a => a.IsSelected, (a) => a)
                .Skip(1)
                .Subscribe(a =>
            {
                var items = Items.OfType<object>().Select(a => this.ItemContainerGenerator.ContainerFromItem(a)).Cast<DragablzItem>().ToArray();
                var selected = items.Where(a => a.IsSelected).Select(a => a.Content).ToArray();
                foreach (var ditem in items)
                {
                    if (ditem != item && ditem.IsSelected == true)
                        ditem.IsSelected = false;
                    else
                    {

                    }
                }
                this.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, selected, new[] { a.Sender.Content }));

            });
            return item;

        }

        class DeleteAdorner : Adorner
        {
            public DeleteAdorner(ItemsControl adornerElement) : base(adornerElement)
            {

            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                if ((AdornedElement is not ItemsControl itemsControl))
                    return;

                var width = itemsControl
                    .Items
                    .OfType<object>()
                    .Select(o =>
                    {
                        var container = itemsControl.ItemContainerGenerator.ContainerFromItem(o);
                        return container;
                    })
                    .OfType<FrameworkElement>()
                    .Sum(a => a.ActualWidth);


                drawingContext.DrawRectangle(Brushes.Red, new Pen(Brushes.White, 1),
                new Rect(new Point(width + 150, 0), new Size(DesiredSize.Height, DesiredSize.Height)));
                //new Rect(new Point(10, DesiredSize.Height - 40), new Size(DesiredSize.Width - 20, 50)));
                base.OnRender(drawingContext);
            }
        }

        class CustomOrganiser : VerticalOrganiser
        {
            public event Action DragCompleted;

            public override void OrganiseOnDragCompleted(DragablzItemsControl requestor, Size measureBounds, IEnumerable<DragablzItem> siblingItems, DragablzItem dragItem)
            {
                DragCompleted?.Invoke();
                base.OrganiseOnDragCompleted(requestor, measureBounds, siblingItems, dragItem);
            }
        }
    }
}
