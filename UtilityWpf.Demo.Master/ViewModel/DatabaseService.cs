using System;
using System.Collections;
using UtilityInterface.NonGeneric.Database;

namespace UtilityWpf.Demo.Sandbox.Infrastructure
{
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
            return Array.Empty<object>();
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
