﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using artmdv_webapi.Models;
using contracts;
using MongoDB.Bson;
using MongoDB.Driver;

namespace data_access
{
    public class ImagesMongo : MongoBase, IImagesDataAccess
    {
        public ImagesMongo() : base("images")
        {
        }

        public string Create(AstroImage image)
        {
            var document = image.ToBsonDocument();
            Collection.InsertOne(document);
            return image.Id;
        }

        public Image Get(string id)
        {
            return GetByFileName(id);
        }

        public new IList<AstroImage> GetAll()
        {
            var images = Collection.Find(FilterDefinition<BsonDocument>.Empty).As<AstroImage>().ToList();
            return images;
        }

        public void Delete(string id)
        {
            Collection.DeleteOne(new {_id = id}.ToBsonDocument());
        }

        public void DeleteImage(string id)
        {
            base.Delete(ObjectId.Parse(id));
        }

        public string SaveImage(Stream fileStream, string filename)
        {
            return UploadFile(fileStream, filename);
        }
    }
}