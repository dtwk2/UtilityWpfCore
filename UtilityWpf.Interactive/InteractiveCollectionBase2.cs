using DynamicData;
using DynamicData.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.NonGeneric;
using UtilityWpf.Interactive.Abstract;
using UtilityWpf.Property;
using UtilityWpf.Interactive.Model;

namespace UtilityWpf.Interactive
{
    public abstract class InteractiveCollectionBase : NPC
    {
        private ISubject<KeyValuePair<IObject, ListChangeReason>> changes = new Subject<KeyValuePair<IObject, ListChangeReason>>();

        protected ReadOnlyObservableCollection<IObject> items;

        public InteractiveCollectionBase()
        {
            Init();
            Changes.Subscribe(_ =>
            {
            });
        }

        private void Init()
        {
            Interactions.Where(_ => _.Value.Interaction == Interaction.Check && (bool?)_.Value.Value == true).Subscribe(_ =>
              {
                  @checked.Add(_.Key);
                  if (@unchecked.Contains(_.Key))
                      @unchecked.Remove(_.Key);
              });
            ChildSubject.Where(_ => _.Value.Interaction == Interaction.Check && (bool?)_.Value.Value == true).Subscribe(_ =>
              {
                  @checked.Add(_.Key);
                  if (@unchecked.Contains(_.Key))
                      @unchecked.Remove(_.Key);
              });
            //              if(!@unchecked.Contains(_.))
            Interactions.Where(_ => _.Value.Interaction == Interaction.Check && !((bool?)_.Value.Value == true)).Subscribe(_ =>
              {
                  @unchecked.Add(_.Key);
                  if (@checked.Contains(_.Key))
                      @checked.Remove(_.Key);
              });
            ChildSubject.Where(_ => _.Value.Interaction == Interaction.Check && !((bool?)_.Value.Value == true)).Subscribe(_ =>
              {
                  @unchecked.Add(_.Key);
                  if (@checked.Contains(_.Key))
                      @checked.Remove(_.Key);
              });
        }

        public string Title { get; protected set; }

        public ICollection<IObject> Items => items;

        public IObservable<KeyValuePair<IObject, ListChangeReason>> Changes => changes;

        public IObservable<KeyValuePair<object, InteractionArgs>> Interactions = new Subject<KeyValuePair<object, InteractionArgs>>();

        public IObservable<UserCommandArgs> UserCommands = new Subject<UserCommandArgs>();

        public ISubject<KeyValuePair<object, InteractionArgs>> ChildSubject = new Subject<KeyValuePair<object, InteractionArgs>>();

        public IObservable<Exception> Errors { get; } = new Subject<Exception>();

        public ObservableCollection<object> @checked { get; } = new ObservableCollection<object>();

        public ObservableCollection<object> @unchecked { get; } = new ObservableCollection<object>();

        public IObservable<object> GetSelectedItem(IObservable<bool> ischecked)
        {
            var ca = this.GetSelected();

            var cb = ChildSubject.Where(_ => _.Value.Interaction == Interaction.Select && (int)_.Value.Value > 0).Select(_ => _.Key);

            var xx = ca.Merge(cb)
                  .CombineLatest(ischecked, (a, b) => new { a, b }).Where(_ => @checked.Contains(_.a) || _.b == false);

            return xx.Select(_ => _.a);
        }

        public IObservable<IList> GetCheckedChildItems(IObservable<bool> ischecked, string childrenpath)
        {
            var x = GetSelectedItem(ischecked);
            return x.Select(_ => ReflectionHelper.RecursivePropValues(_, childrenpath).Cast<object>().Where(a => @checked.Contains(a)).ToList());
        }
    }

    public static class BaseHelper2
    {
        public static void ReactToChanges(this InteractiveCollectionBase col, InteractiveObject so)
        {
            so.WhenPropertyChanged(_ => _.IsSelected)
                .Select(_ => _.Value)
                .Buffer(TimeSpan.FromMilliseconds(250))
                .Where(_ => _.Count > 0)
                .Where(_ => _.All(a => a == true))

         .Subscribe(b =>
         {
             ((ISubject<KeyValuePair<object, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<object, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Select, Value = b.Count }));
         });

            so.WhenPropertyChanged(_ => _.IsExpanded).Select(_ => _.Value).Subscribe(_ =>
             {
                 if (_ != null)
                 {
                     ((ISubject<KeyValuePair<object, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<object, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Expand, Value = _ }));
                     ((ISubject<KeyValuePair<IObject, ChangeReason>>)col.Changes).OnNext(new KeyValuePair<IObject, ChangeReason>(so, ChangeReason.Update));
                 }
             });

            so.Deletions.Subscribe(_ =>
      {
          ((ISubject<UserCommandArgs>)col.UserCommands).OnNext(new UserCommandArgs { UserCommand = UserCommand.Delete, Parameter = so.Object });
      });

            so.WhenPropertyChanged(_ => _.IsChecked).StartWith(new PropertyValue<InteractiveObject, bool?>(so, so.IsChecked)).Subscribe(_ =>
              {
                  if (_.Value != null)
                  {
                      ((ISubject<KeyValuePair<object, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<object, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Check, Value = _.Value }));
                      ((ISubject<KeyValuePair<IObject, ChangeReason>>)col.Changes).OnNext(new KeyValuePair<IObject, ChangeReason>(so, ChangeReason.Update));
                  }
              });
        }

        public static IObservable<object> GetDoubleClicked(this InteractiveCollectionBase bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Select && _.Value.Value.Equals(2)).Select(_ => _.Key);
        }

        public static IObservable<object> GetSelected(this InteractiveCollectionBase bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Select && _.Value.Value.Equals(1)).Select(_ => _.Key);
        }

        public static IObservable<object> GetExpanded(this InteractiveCollectionBase bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Expand && _.Value.Value.Equals(true)).Select(_ => _.Key);
        }

        public static IObservable<object> GetCollapsed(this InteractiveCollectionBase bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Expand && _.Value.Value.Equals(false)).Select(_ => _.Key);
        }

        public static IObservable<object> GetChecked(this InteractiveCollectionBase bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Check && _.Value.Value.Equals(true)).Select(_ => _.Key);
        }

        public static IObservable<object> GetUnChecked(this InteractiveCollectionBase bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Check && _.Value.Value.Equals(false)).Select(_ => _.Key);
        }

        public static IObservable<(bool isChecked, object obj)> SelectCheckedChanges(this InteractiveCollectionBase bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Check).Select(_ => ((bool)_.Value.Value, _.Key));
        }

        public static IObservable<object> GetRemoved(this InteractiveCollectionBase bse)
        {
            return bse.UserCommands.Where(_ => _.UserCommand == UserCommand.Delete).Select(_ => _.Parameter);
        }

        public static IObservable<object> GetAdded(this InteractiveCollectionBase bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Include && _.Value.Value.Equals(true)).Select(_ => _.Key);
        }
    }

    public interface ICollectionViewModel
    {
        ICollection<object> Items { get; }
    }
}