using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using NetFabric.Hyperlinq;
using UtilityHelper;
using UtilityInterface.NonGeneric.Database;
using static UtilityWpf.Service.LiteDbRepository;

namespace UtilityWpf.Service
{
    public class LiteDbRepository : IDatabaseService
    {

        public record ConnectionSettings(Type Type, FileInfo FileInfo, string IdProperty);

        private string _key => Settings.IdProperty;
        //private readonly Settings settings;

        //private string _directory;
        //private ILiteCollection<BsonDocument> collection => cfollection.Value.Item1;
        //private IDisposable _disposable => cfollection.Value.Item2;
        private BsonMapper _mapper = new BsonMapper();


        //private Type type =

        IDisposable GetCollection(out ILiteCollection<BsonDocument> collection)
        {
            collection = LiteDbHelper.GetCollection(Settings, out var _disposable);
            collection.EnsureIndex(a => a[_key]);
            return _disposable;

        }

        public LiteDbRepository(ConnectionSettings settings)
        {
            var fileInfo = settings.FileInfo;
            fileInfo.Directory.Create();
            //cfollection = new Lazy<(ILiteCollection<BsonDocument>, IDisposable)>(() =>
            // {
            //     var ac = LiteDbHelper.GetCollection(settings, out var _disposable);
            //     ac.EnsureIndex(a => a[_key]);
            //     return (ac, _disposable);
            // });

            Settings = settings;
        }


        public ConnectionSettings Settings { get; }


        //public IConvertible GetKey(object trade)
        //{
        //    return (PropertyHelper.GetPropertyValue<IConvertible>(trade, _key.ToString()));
        //}

        protected virtual BsonDocument Convert(object obj)
        {

            var doc = _mapper.ToDocument(obj.GetType(), obj);
            return doc;

        }

        protected virtual object ConvertBack(BsonDocument document)
        {
            var doc = _mapper.ToObject(Settings.Type, document);
            return doc;
        }

        protected virtual IEnumerable<BsonDocument> Convert(IEnumerable<object> objs)
        {
            //var doc = _mapper.ToD(obj.GetType(), objs);
            return objs.Select(obj => Convert(obj));
        }


        public bool Insert(object item)
        {
            using (GetCollection(out var collection))
            {
                collection.Insert(Convert(item));
                return true;
            }
        }

        public bool Update(object item)
        {
            using (GetCollection(out var collection))
            {
                collection.Update(Convert(item));
                return true;
            }
        }

        public bool Delete(object item)
        {
            using (GetCollection(out var collection))
            {
                var cvt = Convert(item);
                var query = Query.EQ("_id", cvt["_id"]);
                var select = collection.Find(query).ToArray();
                if (select.Length != 1)
                {
                    throw new ApplicationException($"Expected length from query,{select.Length}, does not match one.");
                }
                return collection.Delete(cvt["_id"]);
            }
        }

        public int InsertBulk(IList<object> items)
        {
            using (GetCollection(out var collection))
            {
                return collection.InsertBulk(Convert(items));
            }
        }



        public IEnumerable SelectAll()
        {
            using (GetCollection(out var collection))
            {
                return collection.FindAll().Select(a => ConvertBack(a)).ToArray();
            }
        }

        //public IEnumerable SelectAll(object obj)
        //{
        //    return collection.Value.FindAll();
        //}

        public object Select(object item)
        {
            using (GetCollection(out var collection))
            {
                return collection.FindById(new BsonValue(item.GetPropertyValue<IConvertible>(_key, typeof(object))));
            }
        }

        public object SelectById(object item)
        {
            using (GetCollection(out var collection))
            {
                return collection.FindById(new BsonValue(item));
            }
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

        public void Dispose()
        {
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

        public static ILiteCollection<BsonDocument> GetCollection(ConnectionSettings settings, out IDisposable disposable)
        {
            var db = new LiteDatabase(settings.FileInfo.FullName);
            disposable = db;

            // var name = db.GetCollectionNames().SingleOrDefault(a => a.Equals(settings.Type.Name));
            //  if (name != default)
            return db.GetCollection<BsonDocument>(settings.Type.Name);
            //return db.GetCollection("Default");
            //throw new Exception($"Collection does not exist in collection, {name}");
        }


        //public static ILiteCollection<object> GetCollection(string directory, string name, out IDisposable disposable)
        //{
        //    disposable = new LiteDatabase(directory);
        //    return ((LiteDatabase)disposable).GetCollection<object>();
        //}
    }
}
