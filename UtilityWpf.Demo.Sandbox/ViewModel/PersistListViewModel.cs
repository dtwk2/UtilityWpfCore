using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Controls;
using LiteDB;
using ReactiveUI;
using UtilityInterface.NonGeneric.Database;
using UtilityWpf.Demo.Sandbox.ViewModel;
using UtilityWpf.TestData.Model;
using static UtilityWpf.Controls.MasterControl;

namespace UtilityWpf.Demo.Sandbox.Infrastructure
{
    public class PersistListViewModel:ReactiveObject
    {
        private readonly FieldsFactory factory = new();
        private IDatabaseService dbS = new DatabaseService();

        public PersistListViewModel()
        {
            ChangeCommand = ReactiveUI.ReactiveCommand.Create<object, Unit>((a) =>
            {
                switch (a)
                {
                    case MovementEventArgs eventArgs:
                        foreach (var item in eventArgs.Changes)
                        {
                            Data.Move(item.OldIndex, item.Index);
                        }
                        break;
                    default:
                        break;
                }
                return Unit.Default;
            });

            ChangeRepositoryCommand = ReactiveCommand.Create<object, Unit>((a) =>
            {

                if (DatabaseService is LiteDbRepo)
                    DatabaseService = new DatabaseService();
                else 
                    DatabaseService = new LiteDbRepo("../../../Data");


                return Unit.Default;
            });
        }

        public ObservableCollection<Fields> Data => new(factory.Build(5));


        public Fields NewItem => factory.Build(1).Single();

        public ReactiveCommand<object, Unit> ChangeCommand { get; }
        public ReactiveCommand<object, Unit> ChangeRepositoryCommand { get; }

        public IDatabaseService DatabaseService { get=> dbS; private set => this.RaiseAndSetIfChanged(ref dbS, value); } 
    }



    public class DatabaseService : IDatabaseService
    {
        public bool Delete(object item)
        {
            System.Windows.MessageBox.Show("Delete");
            return true;
        }

        public int DeleteBulk(IEnumerable item)
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(object item)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }

        public bool Insert(object item)
        {
            System.Windows.MessageBox.Show("Insert");
            return true;
        }

        public int InsertBulk(IEnumerable item)
        {
            throw new NotImplementedException();
        }

        public object Select(object item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable SelectAll()
        {
            throw new NotImplementedException();
        }

        public object SelectById(object item)
        {
            throw new NotImplementedException();
        }

        public bool Update(object item)
        {
            throw new NotImplementedException();
        }

        public int UpdateBulk(IEnumerable item)
        {
            throw new NotImplementedException();
        }
    }

}
