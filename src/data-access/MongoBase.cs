using System.IO;
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

        protected ObjectId UploadFile(Stream fileStream, string filename)
        {
            var gridFs = new GridFSBucket(_database);
            var id = gridFs.UploadFromStream(filename, fileStream);
            return id;
        }
    }
}
