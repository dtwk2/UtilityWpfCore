using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;
using Evan.Wpf;
using UtilityWpf.Property;

namespace UtilityWpf.View
{
    public class Controlx : Control
    {
        private readonly Dictionary<string, ISubject<object>> subjects = new Dictionary<string, ISubject<object>>();
        private readonly ISubject<DependencyObject> controlChanges = new Subject<DependencyObject>();
        readonly object lck = new object();
        //public List<string> ControlNames = new List<string>();
        // private Lazy<FrameworkElement[]> elements = null;

        //static Controlx()
        //{
        //    //DefaultStyleKeyProperty.OverrideMetadata(typeof(Controlx), new FrameworkPropertyMetadata(typeof(Controlx)));
        //}

        public Controlx()
        {
            foreach (var dp in ReflectionHelper.SelectDependencyPropertiesDeclaredOnly(this.GetType()))
            {
                if (dp.Name == "Id")
                {
                    dp.AddValueChanged(this, (sender, _) => subjects.OnNext(dp.Name, (sender as DependencyObject).GetValue(dp)));
                }
                //dp.AddValueChanged(this, eventHandler);
            }
        }

        public override void OnApplyTemplate()
        {
            foreach (var xx in this.FindVisualChildren<FrameworkElement>())
                controlChanges.OnNext(xx);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            var name = e.Property.Name;
            lock (lck)
            {
                subjects[name] = subjects.ContainsKey(name) ? subjects[name] : new Subject<object>();
            }
            subjects[name].OnNext(e.NewValue);

            base.OnPropertyChanged(e);
        }


        protected IObservable<T> SelectControlChanges<T>(string name = null) where T : FrameworkElement
        {
            return name == null ?
                controlChanges.OfType<T>().Select(a => (T)a) :
                controlChanges.OfType<T>().Select(a => (T)a).Where(a => (a as FrameworkElement).Name == name);
        }


        protected IObservable<object> SelectChanges(string name)
        {
            lock (lck)
            {
                subjects[name] = subjects.ContainsKey(name) ? subjects[name] : new Subject<object>();
                return subjects[name];
            }
        }

        protected IObservable<T> SelectChanges<T>(string name = null)
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
            lock (lck)
            {
                subjects[name] = subjects.ContainsKey(name) ? subjects[name] : new Subject<object>();
                return subjects[name].Select(x => (T)x);
            }
        }

        private async void OnNext(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            string name = dependencyPropertyChangedEventArgs.Property.Name;
            object value = dependencyPropertyChangedEventArgs.NewValue;
            lock (lck)
            {
                subjects[name] = subjects.ContainsKey(name) ? subjects[name] : new Subject<object>();
            }
            await System.Threading.Tasks.Task.Run(() =>
            {
                subjects[name].OnNext(value);
            });
        }

        protected static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Controlx).OnNext(e);
        }


        /// <summary>
        /// Whatches for any changes to dependency properties
        /// </summary>
        /// <returns></returns>
        protected IObservable<Dictionary<string, object>> Any()
        {
            return Observable.Create<Dictionary<string, object>>(observer =>
            {
                Dictionary<string, object> dict = new Dictionary<string, object>();
                var sub = new Subject<Dictionary<string, object>>();
                var xx = new List<IDisposable>();
                foreach (var x in subjects)
                {
                    xx.Add(x.Value.Subscribe(_ => { dict[x.Key] = _; sub.OnNext(dict); }));
                }
                return new System.Reactive.Disposables.CompositeDisposable(xx);
            });
        }

    }
}