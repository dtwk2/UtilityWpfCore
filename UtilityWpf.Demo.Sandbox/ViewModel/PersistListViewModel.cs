using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using LiteDB;
using ReactiveUI;
using UtilityInterface.NonGeneric.Database;
using UtilityWpf.Demo.Sandbox.ViewModel;
using UtilityWpf.TestData.Model;
using static UtilityWpf.Controls.MasterControl;

namespace UtilityWpf.Demo.Sandbox.Infrastructure
{
    public class PersistListViewModel : ReactiveObject
    {
        private readonly FieldsFactory factory = new();
        private IDatabaseService dbS = new DatabaseService();
        //private IReadOnlyCollection<object> data;

        public PersistListViewModel()
        {
            ChangeCommand = ReactiveCommand.Create<object, Unit>((a) =>
            {
                switch (a)
                {
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

            ChangeRepositoryCommand = ReactiveCommand.Create<object, Unit>((a) =>
            {
                if (DatabaseService is LiteDbRepository service)
                {
                    service.Dispose();
                    DatabaseService = new DatabaseService();
                }
                else
                    DatabaseService = new LiteDbRepository(new LiteDbRepository.ConnectionSettings(typeof(Fields), new System.IO.FileInfo("../../../Data/Data.litedb"), nameof(Fields.Id)));


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

        public MintPlayer.ObservableCollection.ObservableCollection<Fields> Data { get; } = new MintPlayer.ObservableCollection.ObservableCollection<Fields>();

        // public ObservableCollection<Fields> Data => new(factory.Build(5));
        //public IReadOnlyCollection<object> Data { get => data; set => this.RaiseAndSetIfChanged(ref data, value); }

        //public IReadOnlyCollection<object> CombinedData { get => data; set => this.RaiseAndSetIfChanged(ref data, value); }


        public IEnumerator NewItem 
        {
            get
            {
                var build = factory.Build();
                return build;
            }
        }

        public ReactiveCommand<object, Unit> ChangeCommand { get; }
        public ReactiveCommand<object, Unit> ChangeRepositoryCommand { get; }
        public ReactiveCommand<object, Unit> CollectionChangedCommand { get; }

        public IDatabaseService DatabaseService { get => dbS; private set => this.RaiseAndSetIfChanged(ref dbS, value); }
    }

}
