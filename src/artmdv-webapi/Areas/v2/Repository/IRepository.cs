using MongoDB.Driver;

namespace artmdv_webapi.Areas.v2.Repository
{
    public interface IRepository
    {
        IMongoDatabase Database { get; set; }
    }
}
