
using System;
using System.Collections;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using UtilityHelper.NonGeneric;
using UtilityWpf.Mixins;
using UtilityWpf.Utility;

namespace UtilityWpf.Controls
{
    public class WrapControl : DoubleContentControl
    {
        public static readonly DependencyProperty ControlsCollectionProperty =
            DependencyProperty.Register("ControlsCollection", typeof(IEnumerable), typeof(WrapControl), new PropertyMetadata(null));

        static WrapControl()
        {
        }

        public WrapControl()
        {
            this.Observable<IEnumerable>(nameof(ControlsCollection)).Subscribe(a =>
            {

            });

            this.Observable<IEnumerable>(nameof(ControlsCollection)).CombineLatest(wrapPanelSubject)
                .Where(a => a.First != null && a.Second != null)
                .Take(1)
                .Subscribe(a =>
                {
                    var (collection, wrapPanel) = a;
                    wrapPanel.Children.Clear();
                    foreach (var item in ControlsCollection.OfType<UIElement>())
                    {
                        wrapPanel.Children.Add(item.Clone());
                    }
                });
        }

        public IEnumerable ControlsCollection
        {
            get { return (IEnumerable)GetValue(ControlsCollectionProperty); }
            set { SetValue(ControlsCollectionProperty, value); }
        }
    }
}

