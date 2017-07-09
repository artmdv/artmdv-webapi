using Microsoft.AspNetCore.Http;

namespace artmdv_webapi.Areas.v2.Models
{
    public class ImageRevisionDto
    {
        public string imageId { get; set; }
        public IFormFile file { get; set; }
        public string description { get; set; }
        public string password { get; set; }
    }
}
