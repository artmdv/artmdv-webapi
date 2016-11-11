using System.IO;

namespace Gallery.Domain.ValueObjects
{
    public class ImageFile
    {
        public string FileName { get; set; }
        public Stream File { get; set; }
    }
}
