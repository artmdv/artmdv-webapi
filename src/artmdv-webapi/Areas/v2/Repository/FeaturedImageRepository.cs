using System;
using artmdv_webapi.Areas.v2.Models;
using MongoDB.Driver;

namespace artmdv_webapi.Areas.v2.Repository
{
    public class FeaturedImageRepository: IFeaturedImageRepository

    {
        private IMongoCollection<FeaturedImage> Collection { get; set; }

        public FeaturedImageRepository()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            Database = client.GetDatabase("v2");
            Collection = Database.GetCollection<FeaturedImage>("FeaturedImages");
        }
        public IMongoDatabase Database { get; set; }

        public FeaturedImage GetLatest()
        {
            var image =  Collection.Find(x => true).SortByDescending(x => x.Date).FirstOrDefault();
            return image;
        }

        public void Save(FeaturedImage image)
        {
            Collection.InsertOne(image);
        }
    }
}
