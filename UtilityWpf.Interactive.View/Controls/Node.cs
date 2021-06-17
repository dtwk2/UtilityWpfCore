using System;
using System.Reactive.Subjects;
using System.Windows.Controls;

namespace UtilityWpf.Interactive.View
{
    public class Node : Control, IObservable<NodeModel> //, IObserver<NodeComponentsEnabledModel>
    {
        public ReplaySubject<NodeModel> nodeModelSubject = new ReplaySubject<NodeModel>();

        public IDisposable Subscribe(IObserver<NodeModel> observer)
        {
            return nodeModelSubject.Subscribe(observer);
        }
    }
}
