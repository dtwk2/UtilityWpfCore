using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using HandyControl.Controls;
using ReactiveUI;
using UtilityInterface.NonGeneric.Database;
using UtilityWpf.TestData.Model;
using static UtilityWpf.Controls.MasterControl;

namespace UtilityWpf.Demo.Sandbox.Infrastructure
{
    public class PersistListViewModel
    {
        private readonly FieldsFactory factory = new();

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
        }

        public ObservableCollection<Fields> Data => new(factory.Build(5));


        public Fields NewItem => factory.Build(1).Single();

        public ReactiveCommand<object, Unit> ChangeCommand { get; }

        public DatabaseService DatabaseService { get; } = new DatabaseService();
    }


    public class DatabaseService : IDatabaseService
    {
        public bool Delete(object item)
        {
            MessageBox.Show("Delete");
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
            MessageBox.Show("Insert");
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
