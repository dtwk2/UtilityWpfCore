using DynamicData.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using UtilityHelper;
using UtilityInterface.Generic;
using UtilityWpf.Abstract;
using UtilityWpf.Interactive.Abstract;
using UtilityWpf.ViewModel;

namespace UtilityWpf.Interactive.Model
{
    //public interface IChildren<T>
    //{
    //    IEnumerable<T> Children { get; set; }

    //}

    //// Selectable / Expandable
    public class ChildInteractiveObject<T> : InteractiveObject<T>
    {
        private InteractiveCollectionViewModel<T> collection;

        public ChildInteractiveObject(T @object, string childrenpath, IObservable<bool> ischecked, IObservable<bool> expand, IConvertible id = null) : base(@object, null, null, id: id)
        {
            Object = @object;
            bool hasinterface = false;
            var type = @object.GetType();
            if (type.GetInterfaces().Contains(typeof(IParent<T>)))
                hasinterface = true;

            expand/*.CombineLatest(ischecked,(a,b)=>new { a, b })*/.Subscribe(_ =>
            {
                // wait until last possible moment before fully initialising class;
                if (type.GetInterfaces().Contains(typeof(IDelayedConstructor)))
                    (@object as IDelayedConstructor).Init(null)
                    .ToObservable()
                    .Subscribe(_a =>
                    privatemethod(@object, childrenpath, ischecked, expand, hasinterface));
                else
                    privatemethod(@object, childrenpath, ischecked, expand, hasinterface);
            });

            ischecked.Subscribe(_ => IsChecked = _);
            // Needs to be run outside of constructor to ensure notfication raised;
            //Init(ischecked);
        }

        private void privatemethod(T @object, string childrenpath, IObservable<bool> ischecked, IObservable<bool> expand, bool hasinterface)
        {
            IEnumerable<T> children = null;
            if (hasinterface)
                children = (@object as IParent<T>).Children;
            else
                children = @object.GetPropertyValue<IEnumerable>(childrenpath)?.Cast<T>();

            if (children != null)
            {
                collection = new InteractiveCollectionViewModel<T>(children, childrenpath, ischecked, this.WhenPropertyChanged(a => a.IsExpanded).Select(b => b.Value).Where(c => c == true).Select(a => (bool)a).Take(1));
                collection.ChildSubject.Subscribe(_a => ChildChanged = _a);
                collection.Interactions/*.Merge(collection.ChildSubject)*/.Subscribe(_a => ChildChanged = _a);
                OnPropertyChanged(nameof(Children));
            }
        }

        //public void Init(IObservable<bool> @checked)
        //{
        //    @checked.Subscribe(_ => IsChecked = _);
        //}

        public ICollection<IObject<T>> Children => collection?.Items ?? new System.Collections.ObjectModel.ReadOnlyObservableCollection<IObject<T>>(new System.Collections.ObjectModel.ObservableCollection<IObject<T>>());

        private KeyValuePair<T, InteractionArgs> _affectedchild;

        public virtual KeyValuePair<T, InteractionArgs> ChildChanged
        {
            get { return _affectedchild; }
            set
            {
                //if (value != _isSelected)
                //{
                _affectedchild = value;
                OnPropertyChanged(nameof(ChildChanged));
                //IsSelected = true;
                //}
            }
        }

        public bool HasItems => (collection?.Items).Count > 0;

        //public T Object { get; private set; }
    }
}