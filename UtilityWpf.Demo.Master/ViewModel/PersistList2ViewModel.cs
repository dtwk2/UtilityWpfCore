using System;
using System.Collections;
using System.Globalization;
using System.Reactive;
using System.Windows.Data;
using AutoMapper;
using ReactiveUI;
using Utility.Common.EventArgs;
using UtilityInterface.NonGeneric.Database;
using UtilityWpf.Demo.Common.ViewModel;
using UtilityWpf.Service;
using UtilityWpf.Demo.Data.Model;
using DynamicData;
using Utility.Common.Enum;
using System.Collections.Generic;
//using UtilityWpf.Model;
using Autofac.Core;
using Utility.Common;

namespace UtilityWpf.Demo.Master.Infrastructure
{
    public class PersistList2ViewModel : ReactiveObject
    {
        private readonly FieldsFactory factory = new();

        private IDatabaseService databaseService = new LiteDbRepository(new LiteDbRepository.ConnectionSettings(typeof(Fields), new System.IO.FileInfo("../../../Data/Data.litedb"), nameof(Fields.Id)));
        private IEnumerator<Fields> build;

        private readonly PersistService service = new();

        public PersistList2ViewModel()
        {         
            service.OnNext(new(databaseService));

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
        public ReactiveCommand<object, Unit> ChangeRepositoryCommand { get; }
        public ReactiveCommand<object, Unit> CollectionChangedCommand { get; }

        //public IDatabaseService DatabaseService { get => dbS; private set => this.RaiseAndSetIfChanged(ref dbS, value); }

    }
}
