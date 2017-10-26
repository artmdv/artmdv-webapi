using System.Threading.Tasks;

namespace artmdv_webapi.Areas.v2.CommandHandlers
{
    public interface IHandler<in TCommand, TResponse>
    {
        Task<TResponse> HandleAsync(TCommand model);
    }
}
