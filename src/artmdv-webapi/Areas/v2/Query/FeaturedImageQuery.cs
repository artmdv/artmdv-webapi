using System.Threading.Tasks;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Repository;

namespace artmdv_webapi.Areas.v2.Query
{
    public class FeaturedImageQuery: IQuery<FeaturedImageViewModel, QueryFilter>
    {
        public IFeaturedImageRepository Repository { get; }

        public FeaturedImageQuery(IFeaturedImageRepository repository)
        {
            Repository = repository;
        }

        public async Task<FeaturedImageViewModel> Get(QueryFilter filter)
        {
            var featuredImage = Repository.GetLatest();
            return await Task.FromResult(new FeaturedImageViewModel
            {
                Date = featuredImage.Date,
                ImageId = featuredImage.ImageId.ToString()
            }).ConfigureAwait(false);

        }
    }
}
