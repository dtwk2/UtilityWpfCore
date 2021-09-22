using System.Collections;
using System.Reactive;
using ReactiveUI;
using Utility.Common.EventArgs;
using UtilityInterface.NonGeneric.Database;
using UtilityWpf.Demo.Common.ViewModel;
using UtilityWpf.Service;
using UtilityWpf.Demo.Data.Model;
using Utility.Common.Enum;
using System.Collections.Generic;
using Utility.Persist;

namespace UtilityWpf.Demo.Master.Infrastructure
{
    public class MasterDetailViewModel : ReactiveObject
    {

        private IEnumerator<Fields> build;

        private readonly CollectionService service = new();

        public MasterDetailViewModel()
        {
            service.OnNext(new(DatabaseService()));

            ChangeCommand = ReactiveCommand.Create<CollectionEventArgs, Unit>((obj) =>
            {
                Change(obj);
                return Unit.Default;
            });
        }

        private void Change(CollectionEventArgs obj)
        {
            switch (obj)
            {
                case { EventType: EventType.Add }:
                    if (NewItem.MoveNext())
                        service.Items.Add(NewItem.Current);
                    break;
                case { EventType: EventType.Remove, Item: { } item }:
                    service.Items.Remove(item);
                    break;
                case { EventType: EventType.Remove }:
                    service.Items.RemoveAt(service.Items.Count - 1);
                    break;
                case MovementEventArgs eventArgs:
                    foreach (var item in eventArgs.Changes)
                    {
                        //Data.Move(item.OldIndex, item.Index);
                    }
                    break;
                default:
                    break;
            }
        }

        public IEnumerable Data => service.Items;

        public IEnumerator NewItem => build ??= Factory().Build();


        public ReactiveCommand<CollectionEventArgs, Unit> ChangeCommand { get; }
        public ReactiveCommand<object, Unit> ChangeRepositoryCommand { get; }
        public ReactiveCommand<object, Unit> CollectionChangedCommand { get; }

        private FieldsFactory Factory() => new();

        IDatabaseService DatabaseService() => new LiteDbRepository(new LiteDbRepository.ConnectionSettings(typeof(Fields), new System.IO.FileInfo("../../../Data/Data.litedb"), nameof(Fields.Id)));

    }
}
