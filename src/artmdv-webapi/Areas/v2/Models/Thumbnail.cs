using artmdv_webapi.Areas.v1.Models;
using MongoDB.Bson;

namespace artmdv_webapi.Areas.v2.Models
{
    public class Thumbnail: IImage
    {
        public ImageType Type => ImageType.Thumb;
        public string Filename { get; set; }
        public string ContentId { get; set; }
    }
}
