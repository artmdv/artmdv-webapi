using artmdv_webapi.Areas.v1.Models;

namespace artmdv_webapi.Areas.v2.Models
{
    public interface IImage
    {
        ImageType Type { get; }
        string Filename { get; set; }
        string ContentId { get; set; }
    }
}
