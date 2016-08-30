using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using artmdv_webapi.Areas.v2.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace artmdv_webapi.Areas.v2.DataAccess
{
    public class ImageDataAccess
    {
        private GridFSBucket GridFs { get; set; }
        public IMongoDatabase Database { get; set; }
        private IMongoCollection<Image> Collection { get; set; }

        private string ImageDirectory { get; set; }

        internal ImageDataAccess()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            Database = client.GetDatabase("v2");
            Collection = Database.GetCollection<Image>("Images");
            GridFs = new GridFSBucket(Database);

            var fs = new FileStream("config.json", FileMode.Open, FileAccess.Read);
            JObject config = null;
            using (StreamReader streamReader = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                config = (JObject)JToken.ReadFrom(reader);
            }
            ImageDirectory = config?.GetValue("ImageDirectory").ToString();
        }

        public string Create(Image image, Stream imagecontent, Stream thumbContent)
        {
            try
            {
                imagecontent.Position = 0;
                var imageId = GridFs.UploadFromStream(image.Filename, imagecontent);
                image.ContentId = imageId.ToString();
                thumbContent.Position = 0;
                var thumbId = GridFs.UploadFromStream(image.Thumb.Filename, thumbContent);
                image.Thumb.ContentId = thumbId.ToString();
                image.Id = ObjectId.GenerateNewId(DateTime.Now);
                Collection.InsertOne(image);

                Directory.CreateDirectory(ImageDirectory);

                using (var fileStream = File.Create($"{ImageDirectory}/{image.Id}{Path.GetExtension(image.Filename)}"))
                {
                    imagecontent.Position = 0;
                    imagecontent.CopyTo(fileStream);
                }
                return image.Id.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public Image Update(Image image)
        {
            var builder = Builders<Image>.Filter;
            var filter = builder.Eq("_id", image.Id);
            Collection.ReplaceOne(filter, image);
            if (!File.Exists($"{ImageDirectory}/{image.Id}.{Path.GetExtension(image.Filename)}"))
            {
                using (var fileStream = File.Create($"{ImageDirectory}/{image.Id}{Path.GetExtension(image.Filename)}"))
                {
                    var imagecontent = GetImageContent(image.Id.ToString());
                    imagecontent.Position = 0;
                    imagecontent.CopyTo(fileStream);
                }
            }
            return image;
        }

        public Image Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
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
            if (string.IsNullOrEmpty(id))
                return null;
            var image = Get(id);

            var content = new MemoryStream();
            GridFs.DownloadToStream(ObjectId.Parse(image.ContentId), content);
            content.Position = 0;
            return content;
        }

        public async Task<List<Image>> GetAllByTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return await Collection.Find(x=>x.Tags != null).ToListAsync();
            }
            return await Collection.Find(x=>x.Tags.Any(y=>tag.Split(',').Contains(y))).ToListAsync();
        }

        public void Delete(string id)
        {
            var image = Get(id);

            GridFs.Delete(ObjectId.Parse(image.ContentId));
            GridFs.Delete(ObjectId.Parse(image.Thumb.ContentId));

            var builder = Builders<Image>.Filter;
            var filter = builder.Eq("_id", ObjectId.Parse(id));
            Collection.DeleteOne(filter);
        }

        public Stream GetAnnotationContent(string id)
        {
            var image = Get(id);
            return GetImageContent(image.Annotation);
        }

        public string GetPath(Image image)
        {
            if (image == null)
                return null;
            if (File.Exists($"{ImageDirectory}/{image.Id}{Path.GetExtension(image.Filename)}"))
            {
                return $"Images/{image.Id}{Path.GetExtension(image.Filename)}";
            }
            return null;
        }

        public string GetAnnotationPath(Image image)
        {
            var annotatedImage = Get(image.Annotation);
            return GetPath(annotatedImage);
        }
    }
}
