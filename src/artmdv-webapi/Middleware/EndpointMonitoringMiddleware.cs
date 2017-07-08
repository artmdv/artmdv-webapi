using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using StatsdClient;

namespace artmdv_webapi.Middleware
{
    public class EndpointMonitoringMiddleware
    {
        private readonly RequestDelegate _next;

        public EndpointMonitoringMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            await _next(context).ConfigureAwait(false);
            stopwatch.Stop();
            
            var routeData = context.GetRouteData();
            if (routeData != null)
            {
                var path = $"{routeData.Values["area"]}.{routeData.Values["controller"]}.{routeData.Values["action"]}";
                Metrics.Timer(path, stopwatch.ElapsedMilliseconds);
            }
            else
            {
                Metrics.Counter("Errors.RouteDataNotFound");
            }
        }
    }
}
