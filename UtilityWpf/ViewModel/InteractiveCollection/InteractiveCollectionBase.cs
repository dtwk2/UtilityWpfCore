using DynamicData;
using DynamicData.Binding;
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


    public abstract class InteractiveCollectionBase<T> : NPC
    {
     

        protected ReadOnlyObservableCollection<IObject<T>> items;

    
        private ISubject<KeyValuePair<IObject<T>, ChangeReason>> changes = new Subject<KeyValuePair<IObject<T>, ChangeReason>>();

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
                  @checked.Add(_.Key);
                  if (@unchecked.Contains(_.Key))
                      @unchecked.Remove(_.Key);
              });
            this.ChildSubject.Where(_ => _.Value.Interaction == Interaction.Check && ((bool?)_.Value.Value == true)).Subscribe(_ =>
              {
                  @checked.Add(_.Key);
                  if (@unchecked.Contains(_.Key))
                      @unchecked.Remove(_.Key);
              });
            //              if(!@unchecked.Contains(_.))
            this.Interactions.Where(_ => _.Value.Interaction == Interaction.Check && !((bool?)_.Value.Value == true)).Subscribe(_ =>
              {
                  @unchecked.Add(_.Key);
                  if (@checked.Contains(_.Key))
                      @checked.Remove(_.Key);
              });
            this.ChildSubject.Where(_ => _.Value.Interaction == Interaction.Check && !((bool?)_.Value.Value == true)).Subscribe(_ =>
              {
                  @unchecked.Add(_.Key);
                  if (@checked.Contains(_.Key))
                      @checked.Remove(_.Key);
              });
        }
        public IObservable<KeyValuePair<T, InteractionArgs>> Interactions = new System.Reactive.Subjects.Subject<KeyValuePair<T, InteractionArgs>>();

        public IObservable<UserCommandArgs> UserCommands = new System.Reactive.Subjects.Subject<UserCommandArgs>();

        public ISubject<KeyValuePair<T, InteractionArgs>> ChildSubject = new Subject<KeyValuePair<T, InteractionArgs>>();

        public IObservable<Exception> Errors { get; } = new Subject<Exception>();

        public string Title { get; protected set; }

        public ICollection<IObject<T>> Items => this.items;

        public IObservable<KeyValuePair<IObject<T>, ChangeReason>> Changes => changes;

        public ObservableCollection<object> @checked { get; } = new ObservableCollection<object>();
       
        public ObservableCollection<object> @unchecked { get; } = new ObservableCollection<object>();

        public IObservable<T> GetSelectedItem(IObservable<bool> ischecked)
        {
            var ca = this.GetSelected();

            var cb = (this as InteractiveCollectionViewModel<T, IConvertible>).ChildSubject.Where(_ => _.Value.Interaction == Interaction.Select && ((int)_.Value.Value) > 0).Select(_ => _.Key);

            var xx = ca.Merge(cb)
                  .CombineLatest(ischecked, (a, b) => new { a, b }).Where(_ => @checked.Contains(_.a) || _.b == false);

            return xx.Select(_ => _.a);
        }

        public IObservable<List<T>> GetCheckedChildItems(IObservable<bool> ischecked, string childrenpath)
        {
            var x = GetSelectedItem(ischecked);
            return x.Select(_ => ReflectionHelper.RecursivePropValues(_, childrenpath).Cast<T>().Where(a => @checked.Contains(a)).ToList());
        }
    }

    //public abstract class InteractiveCollectionBase2<T> : INPCBase
    //{
    //    public IObservable<T> Selected{ get; } = new System.Reactive.Subjects.Subject<T>();

    //    //public IObservable<T> Expanded { get; } = new System.Reactive.Subjects.Subject<T>();

    //    public string Title { get; protected set; }

    //    protected ReadOnlyObservableCollection<SHDObject<T>> _items;

    //    public ICollection<SHDObject<T>> Items => _items;
    //}

    public static class BaseHelper
    {
        public static void ReactToChanges<T>(this InteractiveCollectionBase<T> col, SHDObject<T> so)
        {

            so.WhenPropertyChanged(_ => _.IsSelected).Select(_ => _.Value).Buffer(TimeSpan.FromMilliseconds(250)).Where(_ => _.Count > 0).Where(_ => _.All(a => a == true))
  
         .Subscribe(b =>
         {
                ((System.Reactive.Subjects.ISubject<KeyValuePair<T, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<T, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Select, Value = b.Count }));
         });

            so.WhenPropertyChanged(_ => _.IsExpanded).Select(_ => _.Value).Subscribe(_ =>
             {
                 if (_ != null)
                 {
                     ((ISubject<KeyValuePair<T, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<T, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Expand, Value = _ }));
                     ((ISubject<KeyValuePair<IObject<T>, ChangeReason>>)col.Changes).OnNext(new KeyValuePair<IObject<T>, ChangeReason>(so, ChangeReason.Update));
                 }
             });

     
            so.Deletions.Subscribe(_ =>
            {
                ((ISubject<UserCommandArgs>)col.UserCommands).OnNext(new UserCommandArgs { UserCommand = UserCommand.Delete, Parameter = so.Object });
            });

            so.WhenPropertyChanged(_ => _.IsChecked).StartWith(new PropertyValue<SHDObject<T>, bool?>(so, so.IsChecked)).Subscribe(_ =>
              {
                  if (_.Value != null)
                  {
                      ((System.Reactive.Subjects.ISubject<KeyValuePair<T, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<T, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Check, Value = _.Value }));
                      ((ISubject<KeyValuePair<IObject<T>, ChangeReason>>)col.Changes).OnNext(new KeyValuePair<IObject<T>, ChangeReason>(so, ChangeReason.Update));
                  }
              });
        }


        public static IObservable<T> GetDoubleClicked<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Select && _.Value.Value.Equals(2)).Select(_ => _.Key);
        }

        public static IObservable<T> GetSelected<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Select && _.Value.Value.Equals(1)).Select(_ => _.Key);
        }

        public static IObservable<T> GetExpanded<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Expand && _.Value.Value.Equals(true)).Select(_ => _.Key);
        }

        public static IObservable<T> GetCollapsed<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Expand && _.Value.Value.Equals(false)).Select(_ => _.Key);
        }

        public static IObservable<T> GetChecked<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Check && _.Value.Value.Equals(true)).Select(_ => _.Key);
        }

        public static IObservable<T> GetUnChecked<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Check && _.Value.Value.Equals(false)).Select(_ => _.Key);
        }


        public static IObservable<(bool isChecked,T obj)> SelectCheckedChanges<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Check).Select(_ => ((bool)_.Value.Value, _.Key));
        }

        public static IObservable<T> GetRemoved<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.UserCommands.Where(_ => _.UserCommand == UserCommand.Delete).Select(_ => (T)_.Parameter);
        }

        public static IObservable<T> GetAdded<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Include && _.Value.Value.Equals(true)).Select(_ => _.Key);
        }
    }

    public interface ICollectionViewModel<T>
    {
        ICollection<T> Items { get; }
    }
}