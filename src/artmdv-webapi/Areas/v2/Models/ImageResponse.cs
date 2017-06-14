using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Controllers;

namespace artmdv_webapi.Areas.v2.Models
{
    public class ImageResponse
    {
        public ImageViewModel Image { get; set; }
        public Links links { get; set; }
        public string ForumPost { get; set; }
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
