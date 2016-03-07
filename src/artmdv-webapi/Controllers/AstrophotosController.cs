using System.Collections.Generic;
using System.IO;
using artmdv_webapi.DataAccess;
using artmdv_webapi.Models;
using Microsoft.AspNet.Cors;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ImageResizer;

namespace artmdv_webapi.Controllers
{
    [Route("[controller]")]
    [EnableCors("default")]
    public class AstrophotosController : Controller
    {
        [HttpGet]
        [Route("Image/{id}", Name="ImageRoute")]
        public ActionResult Image(string id)
        {
            var dataAcces = new ImagesMongo();
            Image image = dataAcces.Get(id);
            image.Content.Position = 0;
            return new FileStreamResult(image.Content, "image/jpeg");
        }

        [HttpGet]
        [Route("Thumb/{id}", Name= "ThumbRoute")]
        public ActionResult Thumb(string id)
        {
            var dataAcces = new ImagesMongo();
            Image image = dataAcces.Get(id);
            image.Content.Position = 0;
            var config = new ImageResizer.Configuration.Config();
            var resizedStream = new MemoryStream();
            var job = new ImageJob(image.Content, resizedStream, new Instructions("width=400"));
            config.Build(job);
            resizedStream.Position = 0;
            image.Content = resizedStream;
            return new FileStreamResult(image.Content, "image/jpeg");
        }
        
        [HttpGet]
        public IEnumerable<AstroImage> Get()
        {
            var dataAcces = new ImagesMongo();
            var images = dataAcces.GetAll();
            return images;
        }

        [Route("Migrate")]
        [HttpPost]
        public void Migrate(string id, string title, string description, string password)
        {
            if (CheckPassword(password))
            {
                var dataAcces = new ImagesMongo();
                var image = new AstroImage()
                {
                    Id = id,
                    Image = Url.Link("ImageRoute", new { id = id }),
                    Thumbnail = Url.Link("ThumbRoute", new { id = id }),
                    Title = title,
                    Description = description
                };
                dataAcces.Create(image);
            }
        }

        [HttpPost]
        public dynamic UploadImage(IFormFile file, string title, string description, string password)
        {
            if (CheckPassword(password))
            {
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue
                        .Parse(file.ContentDisposition)
                        .FileName
                        .Trim('"'); // FileName returns "fileName.ext"(with double quotes) in beta 3

                    var dataAcces = new ImagesMongo();
                    var imageId = dataAcces.SaveImage(file.OpenReadStream(), fileName);
                    var image = new AstroImage()
                    {
                        Id = imageId,
                        Image = Url.Link("ImageRoute", new {id = imageId}),
                        Thumbnail = Url.Link("ThumbRoute", new {id = imageId}),
                        Title = title,
                        Description = description
                    };
                    dataAcces.Create(image);
                    return new { Image = image, ForumPost = $"[url={image.Image}][img]{image.Thumbnail}[/img][/url]"};
                }
            }
            return null;
        }

        [HttpDelete]
        public void DeleteImage(string id, string password)
        {
            if (CheckPassword(password))
            {
                var dataAcces = new ImagesMongo();
                dataAcces.Delete(id);
                dataAcces.DeleteImage(id);
            }
        }

        private static bool CheckPassword(string password)
        {
            var fs = new FileStream("config.json", FileMode.Open, FileAccess.Read);
            JObject config = null;
            using (StreamReader streamReader = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                config = (JObject) JToken.ReadFrom(reader);
            }
            return config?.GetValue("password").ToString() == password;
        }
    }
}