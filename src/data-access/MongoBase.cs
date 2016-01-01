using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using contracts;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace data_access
{
    public class MongoBase
    {
        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<BsonDocument> _collection;

        internal MongoBase(string collection)
        {
            _client = new MongoClient("mongodb://localhost:27017");
            _database = _client.GetDatabase("artmdv");
            _collection = _database.GetCollection<BsonDocument>(collection);
        }

        protected string UploadFile(Stream fileStream, string filename)
        {
            var gridFs = new GridFSBucket(_database);
            gridFs.UploadFromStream(filename, fileStream);
            return filename;
        }

        protected Image GetByFileName(string id)
        {
            var gridFs = new GridFSBucket(_database);
            var builders = Builders<GridFSFileInfo>.Filter;
            var filter = builders.Eq("_id", ObjectId.Parse(id));
            var img = gridFs.Find(filter);
            img?.MoveNext();
            var image = new Image();
            image.Id = id;
            image.Filename = img.Current.First().Filename;
            image.Content = new MemoryStream();
            gridFs.DownloadToStream(ObjectId.Parse(image.Id), image.Content);
            return image;
        }

        protected void Delete(ObjectId id)
        {
            var gridFs = new GridFSBucket(_database);
            gridFs.Delete(id);
        }

        protected virtual async Task<IList<Image>> GetAll()
        {
            var gridFs = new GridFSBucket(_database);

            var results = new List<Image>();

            using (var cursor = gridFs.Find(FilterDefinition<GridFSFileInfo>.Empty))
            {
                while (await cursor.MoveNextAsync())
                {
                    foreach (var document in cursor.Current)
                    {
                        var image = new Image();
                        image.Id = document.Id.ToString();
                        image.Filename = document.Filename;
                        image.Content = new MemoryStream();
                        gridFs.DownloadToStream(ObjectId.Parse(image.Id), image.Content);
                        results.Add(image);
                    }
                }
            }
            return results;
        } 
    }
}
