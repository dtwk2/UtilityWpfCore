﻿using ReactiveUI;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reactive;
using Utility.Common.Contract;
using Utility.Common.Enum;
using Utility.Common.EventArgs;
using UtilityInterface.NonGeneric.Data;

namespace Utility.ViewModel
{
    public abstract class MasterDetailViewModel : ReactiveObject
    {
        protected readonly ICollectionService service;
        protected IRepository repository;

        public abstract IEnumerator NewItem { get; }

        public MasterDetailViewModel(ICollectionService service, IRepository repository)
        {
            this.service = service;
            service.OnNext(new(repository));
            ChangeCommand = ReactiveCommand.Create<CollectionEventArgs, Unit>(Switch);
            this.repository = repository;
        }

        private Unit Switch(CollectionEventArgs args)
        {
            switch (args)
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
        }

        public ObservableCollection<object> Data => service.Items;

        public ReactiveCommand<CollectionEventArgs, Unit> ChangeCommand { get; }
    }
}