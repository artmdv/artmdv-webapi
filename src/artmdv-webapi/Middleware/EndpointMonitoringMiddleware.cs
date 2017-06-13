using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
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
            var request = context.Request;
            var path = $"{request.Method}{request.Path.ToString().Replace("/", ".")}";
                
            using (Metrics.StartTimer(path))
            {
                await _next(context).ConfigureAwait(false);
            }
        }

    }
}
