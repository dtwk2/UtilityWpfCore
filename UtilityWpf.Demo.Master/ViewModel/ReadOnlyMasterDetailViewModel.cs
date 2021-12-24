using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using Utility.Common.Enum;
using Utility.Common.EventArgs;
using Utility.Persist;
using Utility.Service;
using UtilityInterface.NonGeneric.Data;
using UtilityWpf.Demo.Common.ViewModel;
using UtilityWpf.Demo.Data.Model;

namespace UtilityWpf.Demo.Master.Infrastructure
{
    public class ReadOnlyMasterDetailViewModel : ReactiveObject
    {
        private readonly ReactiveFieldsFactory factory = new();
        private IRepository dbS = new MockDatabaseService();
        private IEnumerator<ReactiveFields> build;
        private readonly CollectionService service = new();

        public ReadOnlyMasterDetailViewModel()
        {
            this.WhenAnyValue(a => a.DatabaseService)
                .Subscribe(a => { service.OnNext(new(a)); });

            ChangeCommand = ReactiveCommand.Create<CollectionEventArgs, Unit>((obj) =>
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
                return Unit.Default;
            });

            ChangeRepositoryCommand = ReactiveCommand.Create<bool, Unit>((a) =>
            {
                if (DatabaseService is LiteDbRepository service)
                {
                    DatabaseService = new MockDatabaseService();
                }
                else
                    DatabaseService = new LiteDbRepository(new LiteDbRepository.ConnectionSettings(typeof(ReactiveFields), new System.IO.FileInfo("../../../Data/Data.litedb"), nameof(ReactiveFields.Id)));

                return Unit.Default;
            });
        }

        public IEnumerable Data => service.Items;

        public IEnumerator NewItem
        {
            get
            {
                build ??= factory.Build();
                return build;
            }
        }

        public ReactiveCommand<CollectionEventArgs, Unit> ChangeCommand { get; }
        public ReactiveCommand<bool, Unit> ChangeRepositoryCommand { get; }
        public ReactiveCommand<object, Unit> CollectionChangedCommand { get; }

        public IRepository DatabaseService { get => dbS; private set => this.RaiseAndSetIfChanged(ref dbS, value); }
    }
}