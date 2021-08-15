using System;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.Controls
{
    using Mixins;
    using System.Reactive.Linq;

    public abstract class Controlx : Control, IPropertyListener, IControlListener
    {
        private readonly NameTypeDictionary<SingleReplaySubject<object>> dict;

        FrameworkElement[] IControlListener.lazy { get; set; }
        Type IDependencyObjectListener.Type { get; } = typeof(Controlx);
        NameTypeDictionary<SingleReplaySubject<object>> IPropertyListener.dict => dict;

        public Controlx()
        {
            dict = new(this);
      
            this.LoadedChanges()
                .Take(1)
                .Subscribe(a =>
                {
                    (this as IPropertyListener).Init();
                });
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            (this as IPropertyListener).OnPropertyChanged(e);
            base.OnPropertyChanged(e);
        }
    }


}