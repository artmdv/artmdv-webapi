using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using artmdv_webapi.Attributes;
using artmdv_webapi.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Cors;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using contracts;
using data_access;

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
            
            return new FileStreamResult(image.Content, "image/jpeg");
        }
        
        [HttpGet]
        public async Task<IEnumerable<AstroImage>> Get()
        {
            var list = new List<AstroImage>();
            var dataAcces = new ImagesMongo();
            var images = await dataAcces.GetAll();
            foreach (Image image in images)
            {
                list.Add(new AstroImage()
                {
                    Id = image.Id,
                    Image = Url.Link("ImageRoute", new { id = image.Id}),
                    Thumbnail = Url.Link("ThumbRoute", new { id = image.Id }),
                    Title = image.Filename
                });
            }
            return list;
        }
        
        [HttpPost]
        public string UploadImage(IFormFile file, string password)
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
                    return dataAcces.Save(file.OpenReadStream(), fileName);
                }
            }
            return string.Empty;
        }

        [HttpDelete]
        public void DeleteImage(string id, string password)
        {
            if (CheckPassword(password))
            {
                var dataAcces = new ImagesMongo();
                dataAcces.Delete(id);
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