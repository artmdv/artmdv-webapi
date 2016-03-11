using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v1.Models;
using MongoDB.Bson;

namespace artmdv_webapi.Areas.v2.Models
{
    public class ImageViewModel
    {
        public string Id { get; set; }
        public ImageType Type => ImageType.Image;
        public string Filename { get; set; }
        public string ContentId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public Thumbnail Thumb { get; set; }
        public string[] Tags { get; set; }
        public string Date { get; set; }

        public Image ToImage()
        {
            var image = new Image
            {
                Id = ObjectId.Parse(Id),
                Filename = Filename,
                ContentId = ContentId,
                Description = Description,
                Title = Title,
                Thumb = Thumb,
                Tags = Tags,
                Date = Date
            };
            return image;
        }
    }
}
