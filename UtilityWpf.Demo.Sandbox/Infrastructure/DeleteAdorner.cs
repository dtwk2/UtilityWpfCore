using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace UtilityWpf.Demo.View
{
    class DeleteAdorner : Adorner
    {
        public DeleteAdorner(UIElement adornerElement) : base(adornerElement)
        { }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(Brushes.Blue, new Pen(Brushes.Red, 1),
            new Rect(new Point(10, DesiredSize.Height-40), new Size(DesiredSize.Width-20, 50)));
            base.OnRender(drawingContext);
        }
    }
}