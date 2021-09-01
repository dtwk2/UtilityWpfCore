using System.Collections;
using System.Reactive;
using ReactiveUI;
using UtilityInterface.NonGeneric.Database;
using UtilityWpf.Service;
using UtilityWpf.TestData.Model;
using static UtilityWpf.Controls.Master.MasterControl;


namespace UtilityWpf.Demo.Master.Infrastructure
{
    public class PersistListViewModel : ReactiveObject
    {
        private readonly ReactiveFieldsFactory factory = new();
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
                var build = factory.Build();
                return build;
            }
        }

        public ReactiveCommand<object, Unit> ChangeCommand { get; }
        public ReactiveCommand<bool, Unit> ChangeRepositoryCommand { get; }
        public ReactiveCommand<object, Unit> CollectionChangedCommand { get; }

        public IDatabaseService DatabaseService { get => dbS; private set => this.RaiseAndSetIfChanged(ref dbS, value); }
    }

}
