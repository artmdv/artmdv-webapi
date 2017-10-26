using System.Threading.Tasks;

namespace artmdv_webapi.Areas.v2.Query

{
    public interface IQuery<TModel,TFilter>
    {
        Task<TModel> Get(TFilter filter);
    }
}