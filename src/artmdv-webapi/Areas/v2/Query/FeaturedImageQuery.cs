using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Repository;

namespace artmdv_webapi.Areas.v2.Query
{
    public class FeaturedImageQuery: IQuery<FeaturedImageViewModel>
    {
        public IFeaturedImageRepository Repository { get; }

        public FeaturedImageQuery(IFeaturedImageRepository repository)
        {
            Repository = repository;
        }

        public FeaturedImageViewModel Get()
        {
            var featuredImage = Repository.GetLatest();
            return new FeaturedImageViewModel
            {
                Date = featuredImage.Date,
                ImageId = featuredImage.ImageId.ToString()
            };

        }
    }
}
