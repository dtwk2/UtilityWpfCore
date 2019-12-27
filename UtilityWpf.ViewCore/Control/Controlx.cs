using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;


namespace UtilityWpf.View
{
    public class Controlx : Control
    {
        private Dictionary<string, ISubject<object>> Subjects = new Dictionary<string, ISubject<object>>();
        public ISubject<DependencyObject> ControlChanges = new Subject<DependencyObject>();

        public List<string> ControlNames = new List<string>();

        public override void OnApplyTemplate()
        {
            var elements = this.FindVisualChildren<FrameworkElement>().Where(c => string.IsNullOrEmpty(c.Name) == false).ToArray();
            foreach (var element in elements)
                this.ControlChanges.OnNext(element);
            //foreach (string name in ControlNames)
            //{
            //    this.ControlChanges.OnNext(this.GetTemplateChild(name));
            //}
        }

        public IObservable<object> SelectChanges(string name)
        {
            Subjects[name] = Subjects.ContainsKey(name) ? Subjects[name] : new Subject<object>();
            return Subjects[name];
        }

        public IObservable<T> SelectControlChanges<T>() where T : DependencyObject
        {
            var ttype = typeof(T);
            return ControlChanges.Where(a => a.GetType() == ttype).Select(a => (T)a);
        }

        public IObservable<T> SelectChanges<T>(string name = null)
        {
            var type = typeof(T);
            if (string.IsNullOrEmpty(name))
            {
                var props = this.GetType().GetProperties();
                var where = props.Where(a => a.PropertyType == type).ToArray();
                if (where.Any())
                {
                    if (where.Length == 1)
                        name = where.Single().Name;
                    else
                        throw new Exception("UnExpected multiple types");
                }
                else
                    throw new Exception("No types match");
            }

            Subjects[name] = Subjects.ContainsKey(name) ? Subjects[name] : new Subject<object>();
            return Subjects[name].Select(x => (T)x);
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

        public  IObservable<RoutedEventArgs> SelectLoads() =>
            Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>
            (a => this.Loaded += a, a => this.Loaded -= a)
            .Select(a => a.EventArgs);


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