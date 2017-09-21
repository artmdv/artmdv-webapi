using System.IO;
using artmdv_webapi.Areas.v2.Command;
using artmdv_webapi.Areas.v2.CommandHandlers;
using artmdv_webapi.Areas.v2.Commands;
using artmdv_webapi.Areas.v2.Core;
using artmdv_webapi.Areas.v2.Infrastructure;
using artmdv_webapi.Areas.v2.Models;
using artmdv_webapi.Areas.v2.Query;
using artmdv_webapi.Areas.v2.Repository;
using artmdv_webapi.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using StatsdClient;

namespace artmdv_webapi2
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddCors(x => x.AddPolicy("default", y => y.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
            services.AddSingleton<IImageRepository, ImageRepository>();
            services.AddTransient<IFeaturedImageRepository, FeaturedImageRepository>();
            services.AddTransient<IHandler<SetFeaturedImageCommand, object>, SetFeaturedImageHandler>();
            services.AddTransient<IHandler<UploadImageCommand, string>, UploadImageCommandHandler>();
            services.AddTransient<IHandler<UploadImageRevisionCommand, object>, UploadImageRevisionCommandHandler>();
            services.AddTransient<IQuery<FeaturedImageViewModel>, FeaturedImageQuery>();
            services.AddTransient<IFile, LocalFile>();
            services.AddTransient<IDirectory, LocalDirectory>();
            services.AddTransient<ISecurityHandler, SecurityHandler>();
            services.AddTransient<IConfigurationManager, ConfigurationManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), @"Images")),
                RequestPath = new PathString("/Images")
            });

            // Add MVC to the request pipeline.
            app.UseMiddleware<EndpointMonitoringMiddleware>();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute",
                    template: "{area}/{controller}/{action}",
                    defaults: new { area = "v2", controller = "Images", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
            // Add the following route for porting Web API 2 controllers.
            // routes.MapWebApiRoute("DefaultApi", "api/{controller}/{id?}");

            app.UseCors("default");

            app.UseMvc();


            if (env.IsDevelopment())
            {
                Metrics.Configure(new MetricsConfig
                {
                    Prefix = "Development",
                    StatsdServerName = "arturas.space"
                });
            }
            else
            {
                Metrics.Configure(new MetricsConfig
                {
                    Prefix = "Production",
                    StatsdServerName = "localhost"
                });
            }
        }
    }
}
