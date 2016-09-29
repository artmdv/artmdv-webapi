using System.IO;
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
                .UseUrls("http://localhost:5004/")
                .Build();

            host.Run();
        }
    }
}
