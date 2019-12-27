using DynamicData;
using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.Generic;

namespace UtilityWpf.ViewModel
{
    public enum Interaction
    {
        Select, Include, Expand, Check
    }

    public enum UserCommand
    {
        Delete
    }

    public class InteractionArgs : EventArgs
    {
        public Interaction Interaction { get; set; }
        public object Value { get; set; }
    }

    public class UserCommandArgs : EventArgs
    {
        public UserCommand UserCommand { get; set; }
        public object Parameter { get; set; }
    }

    public abstract class InteractiveCollectionBase<T> : NPC
    {
        public IObservable<KeyValuePair<T, InteractionArgs>> Interactions = new System.Reactive.Subjects.Subject<KeyValuePair<T, InteractionArgs>>();

        public IObservable<UserCommandArgs> UserCommands = new System.Reactive.Subjects.Subject<UserCommandArgs>();

        //public IObservable<T> Selected { get; } = new System.Reactive.Subjects.Subject<T>();

        //public IObservable<T> DoubleClicked { get; } = new System.Reactive.Subjects.Subject<T>();

        //public IObservable<T> Deleted { get; } = new System.Reactive.Subjects.Subject<T>();

        //public IObservable<T> Expanded { get; } = new System.Reactive.Subjects.Subject<T>();

        //public IObservable<T> UnChecked { get; } = new System.Reactive.Subjects.Subject<T>();
        public ISubject<KeyValuePair<T, InteractionArgs>> ChildSubject = new Subject<KeyValuePair<T, InteractionArgs>>();

        public IObservable<Exception> Errors { get; } = new Subject<Exception>();

        public string Title { get; protected set; }

        protected ReadOnlyObservableCollection<IContain<T>> items;

        public ICollection<IContain<T>> Items => this.items;

        public IObservable<KeyValuePair<IContain<T>, ChangeReason>> Changes => changes;
        private ISubject<KeyValuePair<IContain<T>, ChangeReason>> changes = new Subject<KeyValuePair<IContain<T>, ChangeReason>>();

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
            //so.IsSelected
            //      .Throttle(TimeSpan.FromMilliseconds(250))
            //         .Subscribe(b =>
            //         {
            //             if (col.Items?.FirstOrDefault(sof =>((SHDObject<T>) sof).IsSelected.Value == true) != (null))
            //                 ((System.Reactive.Subjects.ISubject<T>)col.Selected).OnNext(col.Items.FirstOrDefault(sof => ((SHDObject<T>)sof).IsSelected.Value == true).Object);
            //         });

            so.WhenPropertyChanged(_ => _.IsSelected).Select(_ => _.Value).Buffer(TimeSpan.FromMilliseconds(250)).Where(_ => _.Count > 0).Where(_ => _.All(a => a == true))
         //.Throttle(TimeSpan.FromMilliseconds(250))
         .Subscribe(b =>
         {
             //if (col.Items?.FirstOrDefault(sof => ((SHDObject<T>)sof).IsSelected.Value == true) != (null))
             //{
             //    ((System.Reactive.Subjects.ISubject<T>)col.Selected).OnNext(col.Items.FirstOrDefault(sof => ((SHDObject<T>)sof).IsSelected.Value == true).Object);
             ((System.Reactive.Subjects.ISubject<KeyValuePair<T, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<T, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Select, Value = b.Count }));
         });

            so.WhenPropertyChanged(_ => _.IsExpanded).Select(_ => _.Value).Subscribe(_ =>
             {
                 if (_ != null)
                 {
                     //((System.Reactive.Subjects.ISubject<T>)col.IsExpanded).OnNext(so == default(SHDObject<T>) ? default(T) : so.Object);
                     ((System.Reactive.Subjects.ISubject<KeyValuePair<T, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<T, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Expand, Value = _ }));
                     ((ISubject<KeyValuePair<IContain<T>, ChangeReason>>)col.Changes).OnNext(new KeyValuePair<IContain<T>, ChangeReason>(so, ChangeReason.Update));
                 }
             });

            //so.DoubleClickCommand.Subscribe(_ =>
            //{
            //    ((System.Reactive.Subjects.ISubject<KeyValuePair<T, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<T, InteractionArgs>(_, new InteractionArgs { Interaction = Interaction.Click, Value = 2 }));
            //    //((System.Reactive.Subjects.ISubject<T>)col.DoubleClicked).OnNext(so == default(SHDObject<T>) ? default(T) : so.Object);
            //});

            so.Deletions.Subscribe(_ =>
            {
                ((System.Reactive.Subjects.ISubject<UserCommandArgs>)col.UserCommands).OnNext(new UserCommandArgs { UserCommand = UserCommand.Delete, Parameter = so.Object });
                //((System.Reactive.Subjects.ISubject<T>)col.Deleted).OnNext(so == default(SHDObject<T>) ? default(T) : so.Object);
            });

            so.WhenPropertyChanged(_ => _.IsChecked).StartWith(new PropertyValue<SHDObject<T>, bool?>(so, so.IsChecked)).Subscribe(_ =>
              {
                  if (_.Value != null)
                  {
                      ((System.Reactive.Subjects.ISubject<KeyValuePair<T, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<T, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Check, Value = _.Value }));
                      ((ISubject<KeyValuePair<IContain<T>, ChangeReason>>)col.Changes).OnNext(new KeyValuePair<IContain<T>, ChangeReason>(so, ChangeReason.Update));
                  }
              });
        }

        //public static void ReactToChanges<T>(this InteractiveCollectionBase<T> col, SEObject<T> so)
        //{
        //so.WhenPropertyChanged(_=>_.IsSelected)
        //      .Throttle(TimeSpan.FromMilliseconds(250))
        //         .Subscribe(b =>
        //         {
        //             if (col.Items?.FirstOrDefault(sof => ((SEObject<T>)sof).IsSelected == true) != (null))
        //                 ((System.Reactive.Subjects.ISubject<T>)col.Selected).OnNext(col.Items.FirstOrDefault(sof => ((SEObject<T>)sof).IsSelected== true).Object);
        //         });

        //so.WhenPropertyChanged(_ => _.IsExpanded).Subscribe(_ =>
        // {
        //     ((System.Reactive.Subjects.ISubject<T>)col.Expanded).OnNext(so == default(SEObject<T>) ? default(T) : so.Object);
        // });

        //so.WhenPropertyChanged(_ => _.IsChecked).Subscribe(_ =>
        //{
        //    ((System.Reactive.Subjects.ISubject<T>)col.Checked).OnNext(so == default(SEObject<T>) ? default(T) : so.Object);
        //});

        //so.OnPropertyChange<SEObject<T>,bool>(nameof(SEObject<T>.IsSelected)).Subscribe(_ =>
        //{
        //},(e)=>
        //{ },()=> { });

        //so.WhenPropertyChanged(_ => _.IsSelected).Subscribe(_ =>
        //{
        //});

        //so.DoubleClickCommand.Subscribe(_ =>
        //{
        //    ((System.Reactive.Subjects.ISubject<KeyValuePair<T, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<T, InteractionArgs>(_, new InteractionArgs { Interaction = Interaction.Click, Value = 2 }));
        //    //((System.Reactive.Subjects.ISubject<T>)col.DoubleClicked).OnNext(so == default(SHDObject<T>) ? default(T) : so.Object);
        //});

        //}

        //public static IObservable<T> ReactChecked<T>(this InteractiveCollectionBase<T> bse,SEObject<T> item,IObservable<bool> ischecked)
        //{
        //ischecked.Subscribe(ischecked_ =>
        //{
        //    if (ischecked_)
        //    {
        //        //foreach (var e in enumerable)
        //        //    @checked.AddRange(ReflectionHelper.RecursivePropValues(e, childrenpath).Cast<object>());

        //        @unchecked.Clear();
        //    }
        //    else if (!ischecked_)
        //    {
        //        //foreach (var e in enumerable)
        //        //    @unchecked.AddRange(ReflectionHelper.RecursivePropValues(e, childrenpath).Cast<object>());

        //        @checked.Clear();
        //    }
        //});

        //    .Subscribe(_ =>
        //{
        //});

        //.WithLatestFrom(ischecked, (a, b) => new { a, b })

        //    .Subscribe(_ =>
        //{
        //    if (@checked.Contains(_.a.Key) || _.b == false)
        //    {
        //        this.Dispatcher.InvokeAsync(() => SelectedItem = _.a.Key, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
        //        this.Dispatcher.InvokeAsync(() => SelectedItems = ReflectionHelper.RecursivePropValues(_.a.Key, childrenpath).Cast<object>().Where(a => @checked.Contains(a)).ToList(), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
        //    }
        //});

        //    kx.ChildSubject.Where(_ => _.Value.Interaction == Interaction.Check).Subscribe(_ =>
        //    {
        //        if (!((bool)_.Value.Value))
        //            if (@checked.Contains(_.Key))
        //            {
        //                @checked.Remove(_.Key);
        //                this.Dispatcher.InvokeAsync(() => SelectedItem = null, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
        //                this.Dispatcher.InvokeAsync(() => SelectedItems = null, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
        //            }

        //            else if (((bool)_.Value.Value))
        //                if (@unchecked.Contains(_.Key))
        //                {
        //                    @unchecked.Remove(_.Key);
        //                    this.Dispatcher.InvokeAsync(() => SelectedItem = _.Key, System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
        //                    this.Dispatcher.InvokeAsync(() => SelectedItems = ReflectionHelper.RecursivePropValues(_.Key, childrenpath).Cast<object>().Where(a => @checked.Contains(a)).ToList(), System.Windows.Threading.DispatcherPriority.Background, default(System.Threading.CancellationToken));
        //                }

        //    });
        //}

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

        //public static IObservable<T> GetChecked<T>(this InteractiveCollectionBase<T> bse)
        //{
        //    return bse.@checked.ObserveCollectionChanges().Select(_=>_.);
        //}

        public static IObservable<T> GetUnChecked<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Check && _.Value.Value.Equals(false)).Select(_ => _.Key);
        }


        public static IObservable<(bool,T)> SelectCheckedChanges<T>(this InteractiveCollectionBase<T> bse)
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