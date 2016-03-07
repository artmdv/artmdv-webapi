using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v1.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace artmdv_webapi.Areas.v1.DataAccess
{
    public class MongoBase
    {
        protected readonly MongoClient Client;
        protected readonly IMongoDatabase Database;
        protected readonly IMongoCollection<BsonDocument> Collection;

        internal MongoBase(string collection)
        {
            Client = new MongoClient("mongodb://localhost:27017");
            Database = Client.GetDatabase("artmdv");
            Collection = Database.GetCollection<BsonDocument>(collection);
        }

        protected string UploadFile(Stream fileStream, string filename)
        {
            var gridFs = new GridFSBucket(Database);
            var objectId = gridFs.UploadFromStream(filename, fileStream);
            return objectId.ToString();
        }

        protected Image GetByFileName(string id)
        {
            var gridFs = new GridFSBucket(Database);
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
            var gridFs = new GridFSBucket(Database);
            gridFs.Delete(id);
        }

        protected virtual async Task<IList<Image>> GetAll()
        {
            var gridFs = new GridFSBucket(Database);

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
