using LiteDB;
using NetFabric.Hyperlinq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utility.Common;
using UtilityHelper;
using UtilityInterface.NonGeneric.Data;
using static Utility.Persist.LiteDbRepository;

namespace Utility.Persist
{
    public class LiteDbRepository : IRepository, IIdRepository
    {
        public record ConnectionSettings(Type Type, FileInfo FileInfo, string IdProperty, bool IgnoreBsonDocumentProperties = true);

        private string _key => Settings.IdProperty;

        private BsonMapper _mapper = new BsonMapper();

        private IDisposable GetCollection(out ILiteCollection<BsonDocument> collection)
        {
            collection = LiteDbHelper.GetCollection(Settings, out var _disposable);
            collection.EnsureIndex(a => a[_key]);
            return _disposable;
        }

        public LiteDbRepository(ConnectionSettings settings)
        {
            var fileInfo = settings.FileInfo;
            fileInfo.Directory.Create();
            Settings = settings;
        }

        public ConnectionSettings Settings { get; }

        public object Find(object item)
        {
            using (GetCollection(out var collection))
            {
                return collection.FindById(new BsonValue(item.GetPropertyRefValue<IConvertible>(_key, typeof(object))));
            }
        }

        public object Add(object item)
        {
            using (GetCollection(out var collection))
            {
                collection.Insert(Convert(item));
                return true;
            }
        }

        public object Update(object item)
        {
            using (GetCollection(out var collection))
            {
                collection.Update(Convert(item));
                return true;
            }
        }

        public object Remove(object item)
        {
            using (GetCollection(out var collection))
            {
                var converted = Convert(item);
                var query = Query.EQ("_id", converted["_id"]);
                var select = collection.Find(query).ToArray();
                if (select.Length != 1)
                {
                    throw new ApplicationException($"Expected length from query,{select.Length}, does not match one.");
                }
                return collection.Delete(converted["_id"]);
            }
        }

        public IEnumerable FindMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable IAddMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable UpdateMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public IEnumerable RemoveMany(IEnumerable items)
        {
            throw new NotImplementedException();
        }

        public object FindBy(IQuery query)
        {
            using (GetCollection(out var collection))
            {
                switch (query)
                {
                    case CountQuery:
                        return collection.Count();

                    case FirstQuery:
                        return Convert(collection.Query().First());

                    default:
                        throw new ArgumentOutOfRangeException("789uu7fssd");
                }
            }
        }

        public object AddBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public object UpdateBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public object RemoveBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable AddMany(IEnumerable items)
        {
            using (GetCollection(out var collection))
            {
                var bulk = collection.InsertBulk(Convert(items));
                return Enumerable.Range(0, bulk);
            }
        }

        public IEnumerable AddManyBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable UpdateManyBy(IQuery query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable RemoveMany(IQuery query)
        {
            throw new NotImplementedException();
        }

        public object FindById(long id)
        {
            throw new NotImplementedException();
        }

        public object RemoveById(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable FindManyById(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        public IEnumerable RemoveManyById(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        protected virtual IEnumerable<BsonDocument> Convert(IEnumerable objs)
        {
            //var doc = _mapper.ToD(obj.GetType(), objs);
            return objs.Cast<object>().Select(obj => Convert(obj));
        }

        protected virtual IEnumerable<object> ConvertBack(IEnumerable<BsonDocument> objs)
        {
            //var doc = _mapper.ToD(obj.GetType(), objs);
            return objs.Select(obj => ConvertBack(obj));
        }

        protected virtual BsonDocument Convert(object obj)
        {
            var document = _mapper.ToDocument(obj.GetType(), obj);
            if (Settings.IgnoreBsonDocumentProperties)
                // removes any complex objects since these can't
                // be guaranteed to have parameterless contructor
                foreach (var xx in document)
                {
                    if (xx.Value is BsonDocument)
                    {
                        document.Remove(xx);
                    }
                }
            return document;
        }

        protected virtual object ConvertBack(BsonDocument document)
        {
            if (Settings.IgnoreBsonDocumentProperties)
                // removes any complex objects since these can't
                // be guaranteed to have parameterless contructor
                foreach (var xx in document)
                {
                    if (xx.Value is BsonDocument)
                    {
                        document.Remove(xx);
                    }
                }
            var doc = _mapper.ToObject(Settings.Type, document);
            return doc;
        }

        public IEnumerable FindManyBy(IQuery query)
        {
            using (GetCollection(out var collection))
            {
                switch (query)
                {
                    case AllQuery:
                        return ConvertBack(collection.FindAll()).ToArray();

                    default:
                        throw new ArgumentOutOfRangeException("777fssd");
                }
            }
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