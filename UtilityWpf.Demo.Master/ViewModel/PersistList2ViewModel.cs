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

namespace UtilityWpf.Demo.Master.Infrastructure
{
    public class PersistList2ViewModel : ReactiveObject
    {
        private readonly FieldsFactory factory = new();

        private IDatabaseService dbS = new LiteDbRepository(new LiteDbRepository.ConnectionSettings(typeof(Fields), new System.IO.FileInfo("../../../Data/Data.litedb"), nameof(Fields.Id)));
        private IEnumerator<Fields> build;

        public PersistList2ViewModel()
        {
            ChangeCommand = ReactiveCommand.Create<object, Unit>((a) =>
            {
                switch (a)
                {
                    case CollectionEventArgs { EventType: EventType.Add }:
                        if (NewItem.MoveNext())
                            Data.Add(NewItem.Current as Common.ViewModel.Fields);
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

        public MintPlayer.ObservableCollection.ObservableCollection<Fields> Data { get; } = new MintPlayer.ObservableCollection.ObservableCollection<Fields>();

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

        public IDatabaseService DatabaseService { get => dbS; private set => this.RaiseAndSetIfChanged(ref dbS, value); }

    }
}
