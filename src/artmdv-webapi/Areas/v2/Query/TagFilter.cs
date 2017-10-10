using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace artmdv_webapi.Areas.v2.Query
{
    public class TagFilter: QueryFilter
    {
        public string Tag { get; set; }

        public TagFilter(string tag)
        {
            if (tag == "all")
            {
                tag = string.Empty;
            }
            Tag = tag ?? string.Empty;
        }
    }
}
