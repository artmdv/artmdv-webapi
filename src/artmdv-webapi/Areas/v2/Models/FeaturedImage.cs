using System;
using MongoDB.Bson;

namespace artmdv_webapi.Areas.v2.Models
{
    public class FeaturedImage
    {
        public ObjectId Id { get; set; }
        public ObjectId ImageId { get; set; }
        public DateTime Date { get; set; }
    }
}
