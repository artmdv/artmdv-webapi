using System.IO;

namespace artmdv_webapi.Models
{
    public class Image
    {
        public string Id { get; set; }
        public string Filename { get; set; }
        public Stream Content { get; set; }
    }
}
