//using FreeSql;
//using System;
//using System.Collections.Generic;
//using System.Data.SQLite;
//using System.IO;
//using Utility.Persist.Infrastructure;
//using UtilityInterface.Generic.Data;

//namespace Utility.Persist
//{

//    public class UserGroup : BaseEntity<UserGroup, Guid>
//    {
//        public string UserName { get; set; }
//    }

//    public class SQLiteDataRepository<T, TId, R, S> : IDisposable, IRepository<T, R, S>
//    {
//        private readonly SQLiteConnection conn;

//        public SQLiteDataRepository()
//        {
//            var directory = System.IO.Directory.CreateDirectory("../../../Data");
//            conn = new System.Data.SQLite.SQLiteConnection(@"Data Source=" + Path.Combine(directory.FullName, $"{nameof(T)}.sqlite"));
//            conn.Open();
//            try
//            {
//                UserGroup.Orm.Tab
//                //string createTable = $"CREATE TABLE [{nameof(T)}] ([{nameof(T.Name)}] TEXT NULL)";
//                SqlSugar.Cre
//                var createCommand = new System.Data.SQLite.SQLiteCommand(createTable, conn);
//                createCommand.ExecuteNonQuery();
//            }
//            catch (SQLiteException)
//            {
//            }
//        }

//        public S Add(T item)
//        {
//            throw new NotImplementedException();
//        }

//        public S AddBy(R query)
//        {
//            throw new NotImplementedException();
//        }

//        public S AddMany(IEnumerable<T> items)
//        {
//            throw new NotImplementedException();
//        }

//        public S AddManyBy(R query)
//        {
//            throw new NotImplementedException();
//        }

//        public void Dispose()
//        {
//            conn.Dispose();
//        }

//        public S Find(T item)
//        {
//            throw new NotImplementedException();
//        }

//        public S FindBy(R query)
//        {
//            switch (query)
//            {
//                case MaxRowId:
//                    return new MaxRowIdResult(connection.Table<T>().Length(), true);

//                default:
//                    throw new Exception("SD£vf fdsgff");
//            }
//        }

//        public S FindMany(IEnumerable<T> items)
//        {
//            throw new NotImplementedException();
//        }

//        public S FindManyBy(R query)
//        {
//            throw new NotImplementedException();
//        }

//        public S Remove(T item)
//        {
//            throw new NotImplementedException();
//        }

//        public S RemoveBy(R query)
//        {
//            throw new NotImplementedException();
//        }

//        public S RemoveMany(IEnumerable<T> items)
//        {
//            throw new NotImplementedException();
//        }

//        public S RemoveManyBy(R query)
//        {
//            throw new NotImplementedException();
//        }

//        public S Update(T item)
//        {
//            throw new NotImplementedException();
//        }

//        public S UpdateBy(R query)
//        {
//            throw new NotImplementedException();
//        }

//        public S UpdateMany(IEnumerable<T> items)
//        {
//            throw new NotImplementedException();
//        }

//        public S UpdateManyBy(R query)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
