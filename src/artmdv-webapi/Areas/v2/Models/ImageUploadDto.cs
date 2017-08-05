using Microsoft.AspNetCore.Http;

namespace artmdv_webapi.Areas.v2.Models
{
    public class ImageUploadDto
    {
        public IFormFile file { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string tags { get; set; }
        public string date { get; set; }
        public string annotation { get; set; }
        public string inverted { get; set; }
        public string password { get; set; }
    }
}
