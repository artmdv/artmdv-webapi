using MongoDB.Bson;

namespace artmdv_webapi.Areas.v2.Models
{
    public class Image: IImage
    {
        public ObjectId Id { get; set; }
        public ImageType Type => ImageType.Image;
        public string Filename { get; set; }
        public string ContentId { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public Thumbnail Thumb { get; set; }
        public string Annotation { get; set; }
        public string[] Tags { get; set; }

        public string Date { get; set; }
    }
}
