using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace artmdv_webapi.Areas.v2.Models
{
    public class Revision
    {
        public DateTime RevisionDate { get; set; }
        public string RevisionId { get; set; }
        public string Filename { get; set; }
        public string ContentId { get; set; }
        public Thumbnail Thumb { get; set; }
        public string Description { get; set; }
    }
}
