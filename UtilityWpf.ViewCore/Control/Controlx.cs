using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    public class Controlx : Control
    {
        private Dictionary<string, ISubject<object>> Subjects = new Dictionary<string, ISubject<object>>();

        public ISubject<object> GetSubject(string name)
        {
            Subjects[name] = Subjects.ContainsKey(name) ? Subjects[name] : new Subject<object>(); return Subjects[name];
        }

        public void OnNext(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            string name = dependencyPropertyChangedEventArgs.Property.Name;
            object value = dependencyPropertyChangedEventArgs.NewValue;

            Subjects[name] = Subjects.ContainsKey(name) ? Subjects[name] : new Subject<object>();
            Subjects[name].OnNext(value);
        }

        protected static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Controlx).OnNext(e);
        }

        protected IObservable<Dictionary<string, object>> Any()
        {
            return Observable.Create<Dictionary<string, object>>(observer =>
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                var sub = new Subject<Dictionary<string, object>>();
                var xx = new List<IDisposable>();
                foreach (var x in Subjects)
                {
                    xx.Add(x.Value.Subscribe(_ => { dict[x.Key] = _; sub.OnNext(dict); }));
                }
                return new System.Reactive.Disposables.CompositeDisposable(xx);
            });
        }
    }
}