using System.Collections.Generic;

namespace artmdv_webapi.Areas.v2.Models
{
    public class ImageResponse
    {
        public ImageViewModel Image { get; set; }
        public Links links { get; set; }
    }

    public class Links
    {
        public string ImageContent { get; set; }
        public string ThumbnailContent { get; set; }
        public string AnnotationContent { get; set; }
        public string InvertedContent { get; set; }
        public IList<RevisionPaths> Revisions { get; set; }
    }
}
