using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UtilityWpf.View
{
    //public class ItemsControlx : ItemsControl, IObserver<DependencyPropertyChangedEventArgs>
    //{
    //    Dictionary<string, ISubject<object>> Subjects = new Dictionary<string, ISubject<object>>();

    //    public ISubject<object> GetSubject(string name) => Subjects.GetSubject(name);


    //    protected static void Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        (d as Controlx).OnNext(e);
    //    }

    //    protected IObservable<Dictionary<string, object>> Any() => Subjects.Any();

    //    public void OnNext(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) => Subjects.OnNext(dependencyPropertyChangedEventArgs.Property.Name, dependencyPropertyChangedEventArgs.NewValue);

    //    public void OnError(Exception error) => throw new NotImplementedException();

    //    public void OnCompleted() => throw new NotImplementedException();

    //}
}
