using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using artmdv_webapi.Areas.v2.DataAccess;
using artmdv_webapi.Areas.v2.Models;
using ImageResizer;
using ImageResizer.ExtensionMethods;
using Microsoft.AspNet.Cors;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace artmdv_webapi.Areas.v2.Controllers
{
    [Area("v2")]
    [Route("[area]/Images")]
    [EnableCors("default")]
    public class ImagesController : Controller
    {
        [HttpPost]
        public dynamic UploadImage(IFormFile file, string title, string description, string tags, string password)
        {
            CheckPassword(password);
            
            if (file.Length > 0)
            {
                var fileName = ContentDispositionHeaderValue
                    .Parse(file.ContentDisposition)
                    .FileName
                    .Trim('"'); // FileName returns "fileName.ext"(with double quotes) in beta 3

                var dataAcces = new ImageDataAccess();

                var fileStream = file.OpenReadStream();

                var image = new Image
                {
                    Filename = fileName,
                    Description = description,
                    Title = title,
                    Tags = tags.Split(','),
                    Thumb = new Thumbnail
                    {
                        Filename = $"thumb_{fileName}"
                    }
                };

                var thumb = GenerateThumbnail(fileStream.CopyToMemoryStream());

                var imageId = dataAcces.Save(image, fileStream, thumb);

                return GetImage(imageId.ToString());
            }
            return null;
        }

        [HttpGet]
        [Route("{id}")]
        public dynamic GetImage(string id)
        {
            var dataAcces = new ImageDataAccess();
            var image = dataAcces.Get(id);
            
            return DecorateImage(image);
        }

        private dynamic DecorateImage(Image image)
        {
            var result = new
            {
                Image = image,
                links = new
                {
                    ImageContent = Url.Link("ImageContentRoute", new { image.Id }),
                    ThumbnailContent = Url.Link("ThumbContentRoute", new { image.Id })
                },
                ForumPost = $"[url={Url.Link("ImageContentRoute", new { image.Id })}][img]{Url.Link("ThumbContentRoute", new { image.Id })}[/img][/url]"
            };
            return result;
        }

        [HttpGet]
        public dynamic Getall(string tag=null)
        {
            var dataAcces = new ImageDataAccess();
            var images = dataAcces.GetAllByTag(tag);
            var decoratedImages = new List<dynamic>();
            foreach (var image in images)
            {
                decoratedImages.Add(DecorateImage(image));
            }
            return decoratedImages;
        }

        [HttpGet]
        [Route("{id}/Content", Name = "ImageContentRoute")]
        public ActionResult GetImageContent(string id)
        {
            var dataAcces = new ImageDataAccess();
            var image = dataAcces.GetImageContent(id);
            return new FileStreamResult(image, "image/jpeg");
        }

        [Route("{id}/Thumbnail", Name = "ThumbContentRoute")]
        public ActionResult GetThumb(string id)
        {
            var dataAcces = new ImageDataAccess();
            var image = dataAcces.GetThumbContent(id);
            return new FileStreamResult(image, "image/jpeg");
        }

        private Stream GenerateThumbnail(MemoryStream image)
        {
            image.Position = 0;
            var config = new ImageResizer.Configuration.Config();
            var resizedStream = new MemoryStream();
            var job = new ImageJob(image, resizedStream, new Instructions("width=400"));
            config.Build(job);
            resizedStream.Position = 0;
            return resizedStream;
        }

        private static void CheckPassword(string password)
        {
            var fs = new FileStream("config.json", FileMode.Open, FileAccess.Read);
            JObject config = null;
            using (StreamReader streamReader = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                config = (JObject) JToken.ReadFrom(reader);
            }
            if (config?.GetValue("password").ToString() != password)
            {
                throw new HttpRequestException();
            }

        }
    }
}
