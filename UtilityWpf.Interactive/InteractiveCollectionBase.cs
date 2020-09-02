using DynamicData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.Generic;
using UtilityWpf.Property;

namespace UtilityWpf.ViewModel
{


    public abstract class InteractiveCollectionBase<T> : NPC, IObserver<KeyValuePair<T, InteractionArgs>>
    {
        protected readonly ISubject<KeyValuePair<IObject<T>, ChangeReason>> changes = new Subject<KeyValuePair<IObject<T>, ChangeReason>>();
        protected readonly ISubject<Exception> errors;
        protected readonly ISubject<KeyValuePair<T, InteractionArgs>> childSubject = new Subject<KeyValuePair<T, InteractionArgs>>();
        protected ReadOnlyObservableCollection<IObject<T>> items;
        protected readonly Subject<KeyValuePair<T, InteractionArgs>> interactions = new Subject<KeyValuePair<T, InteractionArgs>>();

        public InteractiveCollectionBase()
        {
            Init();
            Changes.Subscribe(_ =>
            {
            });
        }

        private void Init()
        {
            this.Interactions.Where(_ => _.Value.Interaction == Interaction.Check && ((bool?)_.Value.Value == true)).Subscribe(_ =>
              {
                  Checked.Add(_.Key);
                  if (Unchecked.Contains(_.Key))
                      Unchecked.Remove(_.Key);
              });

            this.ChildSubject.Where(_ => _.Value.Interaction == Interaction.Check && ((bool?)_.Value.Value == true)).Subscribe(_ =>
              {
                  Checked.Add(_.Key);
                  if (Unchecked.Contains(_.Key))
                      Unchecked.Remove(_.Key);
              });

            this.Interactions.Where(_ => _.Value.Interaction == Interaction.Check && !((bool?)_.Value.Value == true)).Subscribe(_ =>
              {
                  Unchecked.Add(_.Key);
                  if (Checked.Contains(_.Key))
                      Checked.Remove(_.Key);
              });

            this.ChildSubject.Where(_ => _.Value.Interaction == Interaction.Check && !((bool?)_.Value.Value == true)).Subscribe(_ =>
              {
                  Unchecked.Add(_.Key);
                  if (Checked.Contains(_.Key))
                      Checked.Remove(_.Key);
              });
        }
        public IObservable<KeyValuePair<T, InteractionArgs>> Interactions => interactions;

        public IObservable<UserCommandArgs> UserCommands { get; } = new Subject<UserCommandArgs>();

        public IObservable<KeyValuePair<T, InteractionArgs>> ChildSubject => childSubject;

        public IObservable<Exception> Errors => errors;

        public string Title { get; protected set; }

        public ICollection<IObject<T>> Items => this.items;

        public IObservable<KeyValuePair<IObject<T>, ChangeReason>> Changes => changes;

        public ObservableCollection<object> Checked { get; } = new ObservableCollection<object>();
       
        public ObservableCollection<object> Unchecked { get; } = new ObservableCollection<object>();

        public IObservable<T> GetSelectedItem(IObservable<bool> ischecked)
        {
            var ca = this.SelectSelected();

            var cb = (this as InteractiveCollectionViewModel<T, IConvertible>).ChildSubject.Where(_ => _.Value.Interaction == Interaction.Select && ((int)_.Value.Value) > 0).Select(_ => _.Key);

            var xx = ca.Merge(cb)
                  .CombineLatest(ischecked, (a, b) => new { a, b }).Where(_ => Checked.Contains(_.a) || _.b == false);

            return xx.Select(_ => _.a);
        }

        public IObservable<List<T>> SelectCheckedChildItems(IObservable<bool> ischecked, string childrenpath)
        {
            var x = GetSelectedItem(ischecked);
            return x.Select(_ => ReflectionHelper.RecursivePropValues(_, childrenpath).Cast<T>().Where(a => Checked.Contains(a)).ToList());
        }

        public void OnCompleted()
        {
            interactions.OnCompleted();
        }

        public void OnError(Exception error)
        {
            interactions.OnError(error);
        }

        public void OnNext(KeyValuePair<T, InteractionArgs> value)
        {
            interactions.OnNext(value);
        }
    }
}