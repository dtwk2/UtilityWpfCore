using System;
using System.Collections;
using UtilityInterface.NonGeneric.Data;

namespace UtilityWpf.Demo.Master.Infrastructure
{
    public class MockDatabaseService : IRepository
    {
        //public bool Delete(object item)
        //{
        //    System.Windows.MessageBox.Show("Delete");
        //    return true;
        //}




        //public bool Insert(object item)
        //{
        //    System.Windows.MessageBox.Show("Insert");
        //    return true;
        //}
        public object Add(object item)
        {
            System.Windows.MessageBox.Show("Insert");
            return "Insert";
        }

        public object AddBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable AddMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable AddManyBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public object Find(object item)
        {
            throw new NotImplementedException();
        }

        public object FindBy(IQuery query)
        {
            return 1;
        }

        public IEnumerable FindMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable FindMany(IQuery query)
        {
            return new[] { "d", "fdfs", "fsddfdd" };
        }

        public object Remove(object item)
        {
            throw new NotImplementedException();
        }

        public object RemoveBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable RemoveMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable RemoveMany(IQuery query)
        {
            throw new NotImplementedException();
        }

        public object Update(object item)
        {
            throw new NotImplementedException();
        }

        public object UpdateBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable UpdateMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable UpdateManyBy(IQuery query)
        {
            throw new NotImplementedException();
        }
    }

}
