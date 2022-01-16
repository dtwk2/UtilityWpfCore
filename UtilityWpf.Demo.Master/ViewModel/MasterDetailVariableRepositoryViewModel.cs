using ReactiveUI;
using System;
using System.Reactive;
using Utility.Persist;
using UtilityInterface.NonGeneric.Data;
using UtilityWpf.Demo.Common.ViewModel;
using UtilityWpf.Demo.Master.Infrastructure;

namespace UtilityWpf.Demo.Master.ViewModel
{
    public class MasterDetailVariableRepositoryViewModel : MasterDetailViewModel
    {
        public MasterDetailVariableRepositoryViewModel() : base()
        {
            this.WhenAnyValue(a => a.DatabaseService)
                .Subscribe(a => { service.OnNext(new(a)); });

            ChangeRepositoryCommand = ReactiveCommand.Create<bool, Unit>((a) =>
            {
                if (DatabaseService is LiteDbRepository service)
                {
                    DatabaseService = new MockDatabaseService();
                }
                else
                    DatabaseService = new LiteDbRepository(new(typeof(ReactiveFields), nameof(ReactiveFields.Id)));

                return Unit.Default;
            });
        }

        public ReactiveCommand<bool, Unit> ChangeRepositoryCommand { get; }

        public IRepository DatabaseService { get => repository; private set => this.RaiseAndSetIfChanged(ref repository, value); }
    }
}