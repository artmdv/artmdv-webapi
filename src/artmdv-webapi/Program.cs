using System.IO;
using artmdv_webapi.Areas.v2.Core;
using Microsoft.AspNetCore.Hosting;

namespace artmdv_webapi2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls($"http://localhost:{ConfigurationManager.GetValue("port")}/")
                .Build();

            host.Run();
        }
    }
}
