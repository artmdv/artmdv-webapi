using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Repository;

namespace artmdv_webapi.Areas.v2.Query
{
    public class FeaturedImageQuery: IQuery<FeaturedImage>
    {
        public IFeaturedImageRepository Repository { get; }

        public FeaturedImageQuery(IFeaturedImageRepository repository)
        {
            Repository = repository;
        }

        public FeaturedImage Get()
        {
            return Repository.GetLatest();
        }
    }
}
