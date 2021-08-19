using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using UtilityHelper;
using UtilityInterface.NonGeneric.Database;

namespace UtilityWpf.Demo.Sandbox.ViewModel
{
    public class LiteDbRepo : IDatabaseService, IDisposable
    {
        private readonly string _key = null;

        //private string _directory;
        private Lazy<ILiteCollection<BsonDocument>> collection;
        private BsonMapper _mapper = new BsonMapper();
        private IDisposable _disposable;

        public LiteDbRepo(string fileName)
        {
            //_key = key;
            //_directory = directory;
            var fi = new FileInfo(fileName);
            fi.Directory.Create();
            collection = new(() => LiteDbHelper.GetCollection(fi, out _disposable));

        }

        public IConvertible GetKey(object trade)
        {
            return (PropertyHelper.GetPropertyValue<IConvertible>(trade, _key.ToString()));
        }

        protected virtual BsonDocument Convert(object obj)
        {
            var doc = _mapper.ToDocument(obj.GetType(), obj);
            return doc;
        }

        protected virtual object ConvertBack(BsonDocument document) {
           var doc = _mapper.ToObject(Type, document);
           return doc;
        }

      protected virtual IEnumerable<BsonDocument> Convert(IEnumerable<object> objs)
        {
            //var doc = _mapper.ToD(obj.GetType(), objs);
            return objs.Select(obj => Convert(obj));
        }


        public bool Insert(object item)
        {
            (collection).Value.Insert(Convert(item));
            return true;
        }

        public bool Update(object item)
        {
            (collection).Value.Update(Convert(item));
            return true;
        }

        public bool Delete(object item)
        {
            (collection).Value.DeleteMany(a => a.GetPropertyValue<IConvertible>(_key, typeof(object)).Equals(item.GetPropertyValue<IConvertible>(_key, typeof(object))));
            return true;
        }

        public int InsertBulk(IList<object> items)
        {
            return collection.Value.InsertBulk(Convert(items));
        }

        public void Dispose()
        {
            _disposable.Dispose();
        }

        public IEnumerable SelectAll()
        {
            return collection.Value.FindAll();
        }

        public object Select(object item)
        {
            return collection.Value.FindById(new LiteDB.BsonValue(UtilityHelper.PropertyHelper.GetPropertyValue<IConvertible>(item, _key, typeof(object))));

        }

        public object SelectById(object item)
        {
            return collection.Value.FindById(new BsonValue(item));
        }

        public int InsertBulk(IEnumerable item)
        {
            throw new NotImplementedException();
        }

        public int UpdateBulk(IEnumerable item)
        {
            throw new NotImplementedException();
        }

        public int DeleteBulk(IEnumerable item)
        {
            throw new NotImplementedException();
        }

        public bool DeleteById(object item)
        {
            throw new NotImplementedException();
        }
    }

    public static class LiteDbHelper
    {
        //public static ILiteCollection<T> GetCollection<T>(DirectoryInfo directory, string name, out IDisposable disposable, string collectionName = null)
        //{
        //    disposable = new LiteDatabase(directory.File / name.AddExtension(Constants.FileExtension.LiteDB).Value);
        //    return ((LiteDatabase)disposable).GetCollection<T>(collectionName);
        //}

        public static ILiteCollection<T> GetCollection<T>(FileInfo file, out IDisposable disposable, string collectionName = null)
        {
            var db = new LiteDatabase(file.FullName);
            disposable = db;

            if (collectionName == null)
                return db.GetCollection<T>();
            if (db.CollectionExists(collectionName))
                return db.GetCollection<T>(collectionName);


            var names = db.GetCollectionNames();
            throw new Exception($"Collection does not exist in collection, {string.Join(", ", names)}");
        }

        public static ILiteCollection<BsonDocument> GetCollection(FileInfo file, out IDisposable disposable, string collectionName = null)
        {
            var db = new LiteDatabase(file.FullName);
            disposable = db;

            var name = db.GetCollectionNames().SingleOrDefault();
            if (name != default)
                return db.GetCollection<BsonDocument>(name);
            return db.GetCollection("Default");
            //throw new Exception($"Collection does not exist in collection, {name}");
        }


        //public static ILiteCollection<object> GetCollection(string directory, string name, out IDisposable disposable)
        //{
        //    disposable = new LiteDatabase(directory);
        //    return ((LiteDatabase)disposable).GetCollection<object>();
        //}
    }
}
