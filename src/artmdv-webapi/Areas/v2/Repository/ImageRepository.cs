﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace artmdv_webapi.Areas.v2.Repository
{
    public class ImageRepository : IImageRepository
    {
        private GridFSBucket GridFs { get; set; }
        public IMongoDatabase Database { get; set; }
        private IMongoCollection<Image> Collection { get; set; }

        private string ImageDirectory { get; set; }

        public ImageRepository()
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
            if (!Directory.Exists(ImageDirectory))
            {
                Directory.CreateDirectory(ImageDirectory);
            }
        }

        public string Create(Image image, Stream imagecontent, Stream thumbContent)
        {
            imagecontent.Position = 0;

            image.Filename = CreateImageFile(imagecontent, image.Filename);

            var thumbId = CreateThumb(image.Thumb.Filename, thumbContent);
            image.Thumb.ContentId = thumbId.ToString();
            image.Id = ObjectId.GenerateNewId(DateTime.Now);
            
            Collection.InsertOne(image);
            
            return image.Id.ToString();
        }

        public string CreateImageFile(Stream imageContent, string fileName)
        {
            var generatedFileName = GenerateFileName(fileName);
            using (var fileStream = File.Create($"{ImageDirectory}/{generatedFileName}"))
            {
                imageContent.Position = 0;
                imageContent.CopyTo(fileStream);
            }

            return generatedFileName;
        }

        public string GenerateFileName(string fileName)
        {
            //TODO: write test
            var newFileName = Path.GetFileNameWithoutExtension(fileName);
            var generatedFileName = newFileName;
            var i = 1;
            while (File.Exists($"{ImageDirectory}/{generatedFileName}{Path.GetExtension(fileName)}"))
            {
                generatedFileName = $"{newFileName}({i++})";
            }
            return $"{generatedFileName}{Path.GetExtension(fileName)}";
        }

        public string CreateThumb(string filename, Stream thumbContent)
        {
            thumbContent.Position = 0;
            return GridFs.UploadFromStream(filename, thumbContent).ToString();
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
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var objId = new ObjectId();
            if (!ObjectId.TryParse(id, out objId))
            {
                return null;
            }
            var builder = Builders<Image>.Filter;
            var filter = builder.Eq("_id", ObjectId.Parse(id));
            return Collection.Find(filter).FirstOrDefault();
        }

        public Stream GetThumbContent(string id)
        {
            var objId = new ObjectId();
            if (!ObjectId.TryParse(id, out objId))
            {
                return null;
            }
            var image = Get(id);

            var content = new MemoryStream();
            GridFs.DownloadToStream(ObjectId.Parse(image.Thumb.ContentId), content);
            content.Position = 0;
            return content;
        }

        public Stream GetByContentId(string id)
        {
            var objId = new ObjectId();
            if (!ObjectId.TryParse(id, out objId))
            {
                return null;
            }
            var content = new MemoryStream();
            GridFs.DownloadToStream(ObjectId.Parse(id), content);
            content.Position = 0;
            return content;
        }

        public Stream GetImageContent(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }

            var objId = new ObjectId();
            if (!ObjectId.TryParse(id, out objId))
            {
                return null;
            }

            var image = Get(id);

            if (string.IsNullOrEmpty(image.ContentId))
            {
                return null;
            }

            var content = new MemoryStream();
            GridFs.DownloadToStream(ObjectId.Parse(image.ContentId), content);
            content.Position = 0;
            return content;
        }

        public async Task<List<Image>> GetAllByTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return await Collection.Find(x=>x.Tags != null).ToListAsync().ConfigureAwait(false);
            }
            return await Collection.Find(x=>x.Tags.Any(y=>tag.Split(',').Contains(y))).ToListAsync().ConfigureAwait(false);
        }

        public void Delete(string id)
        {
            var image = Get(id);
            if (image != null)
            {
                GridFs.Delete(ObjectId.Parse(image.ContentId));
                GridFs.Delete(ObjectId.Parse(image.Thumb.ContentId));

                var builder = Builders<Image>.Filter;
                var filter = builder.Eq("_id", ObjectId.Parse(id));
                Collection.DeleteOne(filter);
            }
        }

        public Stream GetAnnotationContent(string id)
        {
            var image = Get(id);
            return GetImageContent(image.Annotation);
        }

        public Stream GetInvertedContent(string id)
        {
            var image = Get(id);
            return GetImageContent(image.Inverted);
        }

        public string GetPath(Image image)
        {
            if (image == null)
            {
                return null;
            }
            if (File.Exists($"{ImageDirectory}/{image.Filename}"))
            {
                return $"{ImageDirectory}/{image.Filename}";
            }
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

        public string GetInvertedPath(Image image)
        {
            var invertedImage = Get(image.Inverted);
            return GetPath(invertedImage);
        }

        public string GetRevisionPath(Revision image)
        {
            if (image == null)
            {
                return null;
            }
            if (File.Exists($"{ImageDirectory}/{image.Filename}"))
            {
                return $"{ImageDirectory}/{image.Filename}";
            }
            if (File.Exists($"{ImageDirectory}/{image.ContentId}{Path.GetExtension(image.Filename)}"))
            {
                return $"Images/{image.ContentId}{Path.GetExtension(image.Filename)}";
            }
            return null;
        }
    }
}