using System.IO;
using artmdv_webapi.Areas.v2.Core;
using artmdv_webapi.Areas.v2.Infrastructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace artmdv_webapi2
{
    public class Program
    {

        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var cfg = new ConfigurationManager(new LocalFile());
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseUrls($"http://localhost:{cfg.GetValue("port")}/")
                .Build();
        }
    }
}
