using Evan.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using UtilityWpf.Property;

namespace UtilityWpf.Controls
{
    public class ContentControlx : ContentControl
    {
        //private readonly Dictionary<string, ISubject<object>> Subjects = new Dictionary<string, ISubject<object>>();
        //protected readonly ISubject<DependencyObject> ControlChanges = new ReplaySubject<DependencyObject>(1);
        //readonly object lck = new object();
        //public List<string> ControlNames = new List<string>();
        public Dictionary<string, ISubject<object>> Subjects { get; } = new Dictionary<string, ISubject<object>>();

        public ISubject<DependencyObject> ControlChanges { get; } = new ReplaySubject<DependencyObject>(1);

        public ContentControlx()
        {
            foreach (var dp in ReflectionHelper.SelectDependencyPropertiesDeclaredOnly(this.GetType()))
            {
                if (dp.Name == "Id")
                {
                    dp.AddValueChanged(this, (s, e) => eventHandler(s as DependencyObject, dp));
                }
                //dp.AddValueChanged(this, eventHandler);
            }

            void eventHandler(DependencyObject sender, DependencyProperty e)
            {
                Subjects.OnNext(e.Name, sender.GetValue(e));
            }
        }

        public override void OnApplyTemplate()
        {
            var elements = this.FindVisualChildren<FrameworkElement>().Where(c => string.IsNullOrEmpty(c.Name) == false).ToArray();
            foreach (var element in elements)
                (this).ControlChanges.OnNext(element);
        }

        protected static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ContentControlx ccx)
                ccx.OnNext(e);
            else
            {
                throw new Exception("Incorrect type " + d.GetType() + ". Exptected " + nameof(ContentControlx));
            }
        }

        public IObservable<object> SelectChanges(string name)
        {
            lock (Subjects)
            {
                Subjects[name] = Subjects.ContainsKey(name) ? Subjects[name] : new ReplaySubject<object>(1);
                return Subjects[name];
            }
        }

        public IObservable<T> SelectControlChanges<T>(string? name = null) where T : DependencyObject
        {
            var ttype = typeof(T);
            var s = ControlChanges.Where(a => a.GetType() == ttype).Select(a => (T)a);
            return name == null ? s : s.Where(a => (a as FrameworkElement).Name == name);
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
            lock (Subjects)
            {
                Subjects[name] = Subjects.ContainsKey(name) ? Subjects[name] : new ReplaySubject<object>(1);
                return Subjects[name].Select(x => (T)x);
            }
        }

        public async void OnNext(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            string name = dependencyPropertyChangedEventArgs.Property.Name;
            object value = dependencyPropertyChangedEventArgs.NewValue;
            lock (Subjects)
            {
                Subjects[name] = Subjects.ContainsKey(name) ? Subjects[name] : new ReplaySubject<object>(1);
            }
            await System.Threading.Tasks.Task.Run(() =>
            {
                Subjects[name].OnNext(value);
            });
        }

        protected IObservable<Dictionary<string, object>> Any()
        {
            return Observable.Create<Dictionary<string, object>>(observer =>
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                var sub = new ReplaySubject<Dictionary<string, object>>(1);
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