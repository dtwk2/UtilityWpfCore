using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using DynamicData;
using ReactiveUI;
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

        //private IReadOnlyCollection<object> data;

        public PersistListViewModel()
        {
            ChangeCommand = ReactiveCommand.Create<object, Unit>((a) =>
            {
                switch (a)
                {
                    case CollectionEventArgs { EventType: EventType.Add }:
                        if (NewItem.MoveNext())
                            Data.Add(NewItem.Current as Common.ViewModel.ReactiveFields);
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
                    service.Dispose();
                    DatabaseService = new DatabaseService();
                }
                else
                    DatabaseService = new LiteDbRepository(new LiteDbRepository.ConnectionSettings(typeof(ReactiveFields), new System.IO.FileInfo("../../../Data/Data.litedb"), nameof(ReactiveFields.Id)));


                return Unit.Default;
            });

            CollectionChangedCommand = ReactiveCommand.Create<object, Unit>(a =>
            {

                if (a is IEnumerable enumerable)
                {

                    //Data.Clear();
                    //Data.AddRange(enumerable.OfType<Fields>());
                }
                else
                {

                }
                return Unit.Default;

            });

        }

        public MintPlayer.ObservableCollection.ObservableCollection<ReactiveFields> Data { get; } = new MintPlayer.ObservableCollection.ObservableCollection<ReactiveFields>();

        // public ObservableCollection<Fields> Data => new(factory.Build(5));
        //public IReadOnlyCollection<object> Data { get => data; set => this.RaiseAndSetIfChanged(ref data, value); }

        //public IReadOnlyCollection<object> CombinedData { get => data; set => this.RaiseAndSetIfChanged(ref data, value); }


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
