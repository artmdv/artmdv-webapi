using artmdv_webapi.Areas.v2.Models;

namespace artmdv_webapi.Areas.v2.Repository
{
    public interface IFeaturedImageRepository: IRepository
    {
        FeaturedImage GetLatest();
        void Save(FeaturedImage image);
    }
}
