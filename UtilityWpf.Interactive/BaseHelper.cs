using DynamicData;
using DynamicData.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using UtilityInterface.Generic;

namespace UtilityWpf.ViewModel
{
    public static class BaseHelper
    {
        public static void ReactToChanges<T>(this InteractiveCollectionBase<T> col, InteractiveObject<T> so)
        {

            so.WhenPropertyChanged(_ => _.IsSelected).Select(_ => _.Value).Buffer(TimeSpan.FromMilliseconds(250)).Where(_ => _.Count > 0).Where(_ => _.All(a => a == true))
  
         .Subscribe(b =>
         {
                ((ISubject<KeyValuePair<T, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<T, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Select, Value = b.Count }));
         });

            so.WhenPropertyChanged(a => a.IsDoubleClicked)
                .Select(so => new KeyValuePair<T, InteractionArgs>(so.Sender.Object, new InteractionArgs { Interaction = Interaction.DoubleSelect }))
                .Subscribe(col);
//                {
//                    (col.Interactions.OnNext());
//});

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

            so.WhenPropertyChanged(_ => _.IsChecked).StartWith(new PropertyValue<InteractiveObject<T>, bool?>(so, so.IsChecked)).Subscribe(_ =>
              {
                  if (_.Value != null)
                  {
                      ((System.Reactive.Subjects.ISubject<KeyValuePair<T, InteractionArgs>>)col.Interactions).OnNext(new KeyValuePair<T, InteractionArgs>(so.Object, new InteractionArgs { Interaction = Interaction.Check, Value = _.Value }));
                      ((ISubject<KeyValuePair<IObject<T>, ChangeReason>>)col.Changes).OnNext(new KeyValuePair<IObject<T>, ChangeReason>(so, ChangeReason.Update));
                  }
              });
        }


        public static IObservable<T> SelectDoubleClicked<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.DoubleSelect).Select(_ => _.Key);
        }

        public static IObservable<T> SelectSelected<T>(this InteractiveCollectionBase<T> bse)
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

        public static IObservable<T> SelectRemoved<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.UserCommands.Where(_ => _.UserCommand == UserCommand.Delete).Select(_ => (T)_.Parameter);
        }

        public static IObservable<T> GetAdded<T>(this InteractiveCollectionBase<T> bse)
        {
            return bse.Interactions.Where(_ => _.Value.Interaction == Interaction.Include && _.Value.Value.Equals(true)).Select(_ => _.Key);
        }
    }
}