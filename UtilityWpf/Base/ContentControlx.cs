﻿using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Mixins;
using UtilityWpf.Utility;

namespace UtilityWpf.Base
{
    public abstract class ContentControlx : ContentControl, IPropertyListener, IControlListener
    {
        private readonly NameTypeDictionary<SingleReplaySubject<object>> nameTypeDictionary;

        //public NameTypeDictionary<Subject<object>> dict { get; }
        Type IDependencyObjectListener.Type { get; } = typeof(ContentControlx);

        //public INameTypeDictionary NameTypeDictionary { get; }
        NameTypeDictionary<SingleReplaySubject<object>> IPropertyListener.dict => nameTypeDictionary;

        IObservable<FrameworkElement>? IControlListener.lazy { get; set; }

        protected ContentControlx()
        {
            nameTypeDictionary = new NameTypeDictionary<SingleReplaySubject<object>>(this);
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