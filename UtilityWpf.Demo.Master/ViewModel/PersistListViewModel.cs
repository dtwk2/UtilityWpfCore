﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using ReactiveUI;
using Utility.Common;
using Utility.Common.Enum;
using Utility.Common.EventArgs;
using UtilityInterface.NonGeneric.Database;
using UtilityWpf.Demo.Common.ViewModel;
using UtilityWpf.Demo.Data.Model;
using UtilityWpf.Service;


namespace UtilityWpf.Demo.Master.Infrastructure
{
    public class PersistListViewModel : ReactiveObject
    {
        private readonly ReactiveFieldsFactory factory = new();
        private IDatabaseService dbS = new DatabaseService();
        private IEnumerator<ReactiveFields> build;
        private readonly PersistService service = new();

        public PersistListViewModel()
        {
            this.WhenAnyValue(a => a.DatabaseService)
                .Subscribe(a => { service.OnNext(new(a)); });

            ChangeCommand = ReactiveCommand.Create<object, Unit>((obj) =>
            {
                switch (obj)
                {
                    case CollectionEventArgs { EventType: EventType.Add }:
                        if (NewItem.MoveNext())
                            service.Items.Add(NewItem.Current);
                        break;
                    case CollectionEventArgs { EventType: EventType.Remove, Item: { } item }:
                        service.Items.Remove(item);
                        break;
                    case CollectionEventArgs { EventType: EventType.Remove }:
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
                if (DatabaseService is LiteDbRepository service)                {
                    service.Dispose();
                    DatabaseService = new DatabaseService();
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

        public ReactiveCommand<object, Unit> ChangeCommand { get; }
        public ReactiveCommand<bool, Unit> ChangeRepositoryCommand { get; }
        public ReactiveCommand<object, Unit> CollectionChangedCommand { get; }

        public IDatabaseService DatabaseService { get => dbS; private set => this.RaiseAndSetIfChanged(ref dbS, value); }
    }

}
