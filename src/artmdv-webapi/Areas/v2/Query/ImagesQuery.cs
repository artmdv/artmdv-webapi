using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Repository;

namespace artmdv_webapi.Areas.v2.Query
{
    public class ImagesQuery: IQuery<List<Image>, TagFilter>
    {
        public IImageRepository DataAccess { get; }

        public ImagesQuery(IImageRepository dataAccess)
        {
            DataAccess = dataAccess;
        }

        public async Task<List<Image>> Get(TagFilter filter)
        {
            var images = await DataAccess.GetAllByTag(filter.Tag).ConfigureAwait(false);

            return images.OrderByDescending(x => x.Date).ToList();
        }
    }
}
