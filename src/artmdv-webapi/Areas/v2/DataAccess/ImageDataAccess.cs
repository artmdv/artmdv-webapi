using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using artmdv_webapi.Areas.v2.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace artmdv_webapi.Areas.v2.DataAccess
{
    public class ImageDataAccess
    {
        private GridFSBucket GridFs { get; set; }
        public IMongoDatabase Database { get; set; }
        private IMongoCollection<Image> Collection { get; set; }

        internal ImageDataAccess()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            Database = client.GetDatabase("v2");
            Collection = Database.GetCollection<Image>("Images");
            GridFs = new GridFSBucket(Database);
        }

        public string Create(Image image, Stream imagecontent, Stream thumbContent)
        {
            imagecontent.Position = 0;
            var imageId = GridFs.UploadFromStream(image.Filename, imagecontent);
            image.ContentId = imageId.ToString();
            thumbContent.Position = 0;
            var thumbId = GridFs.UploadFromStream(image.Thumb.Filename, thumbContent);
            image.Thumb.ContentId = thumbId.ToString();
            image.Id = ObjectId.GenerateNewId(DateTime.Now);
            Collection.InsertOne(image);
            return image.Id.ToString();
        }

        public Image Update(Image image)
        {
            var builder = Builders<Image>.Filter;
            var filter = builder.Eq("_id", image.Id);
            Collection.ReplaceOne(filter, image);
            return image;
        }

        public Image Get(string id)
        {
            var builder = Builders<Image>.Filter;
            var filter = builder.Eq("_id", ObjectId.Parse(id));
            return Collection.Find(filter).FirstOrDefault();
        }

        public Stream GetThumbContent(string id)
        {
            var image = Get(id);

            var content = new MemoryStream();
            GridFs.DownloadToStream(ObjectId.Parse(image.Thumb.ContentId), content);
            content.Position = 0;
            return content;
        }

        public Stream GetImageContent(string id)
        {
            var image = Get(id);

            var content = new MemoryStream();
            GridFs.DownloadToStream(ObjectId.Parse(image.ContentId), content);
            content.Position = 0;
            return content;
        }

        public List<Image> GetAllByTag(string tag)
        {
            return Collection.Find(x=>x.Tags.Contains(tag)).ToListAsync().Result;
        }
    }
}
