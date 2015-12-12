using System;
using System.Collections.Generic;

using artmdv_webapi.Attributes;
using artmdv_webapi.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Cors;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Net.Http.Headers;

namespace artmdv_webapi.Controllers
{
    [Route("[controller]")]
    [EnableCors("default")]
    public class AstrophotosController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IEnumerable<AstroImage> Get()
        {
            var list = new List<AstroImage>();
            list.Add(new AstroImage()
            {
                Image = "images/ap/NGC6995.jpg",
                Thumbnail = "images/ap/NGC6995_thumb.jpg",
                Title = "NGC 6995 Eastern Veil nebula"
            });
            list.Add(new AstroImage()
            {
                Image = "images/ap/M16.jpg",
                Thumbnail = "images/ap/M16_thumb.jpg",
                Title = "M16 Eagle nebula"
            });
            list.Add(new AstroImage()
            {
                Image = "images/ap/M51.jpg",
                Thumbnail = "images/ap/M51_thumb.jpg",
                Title = "M51 Whirlpool galaxy"
            });
            list.Add(new AstroImage()
            {
                Image = "images/ap/M57.jpg",
                Thumbnail = "images/ap/M57_thumb.jpg",
                Title = "M57 Ring nebula"
            });
            list.Add(new AstroImage()
            {
                Image = "images/ap/M81_M82.jpg",
                Thumbnail = "images/ap/M81_M82_thumb.jpg",
                Title = "M81 Bode's galaxy and M82 Cigar galaxy"
            });
            list.Add(new AstroImage()
            {
                Image = "images/ap/milky-way.jpg",
                Thumbnail = "images/ap/milky-way_thumb.jpg",
                Title = "Milky Way"
            });
            list.Add(new AstroImage()
            {
                Image = "images/ap/NGC7331.jpg",
                Thumbnail = "images/ap/NGC7331_thumb.jpg",
                Title = "NGC 7331 and friends"
            });
            return list;
        }
        
        [PasswordAuthorize, HttpPost]
        public Guid UploadImage(IFormFile file)
        {
            var fileName = ContentDispositionHeaderValue
                .Parse(file.ContentDisposition)
                .FileName
                .Trim('"'); // FileName returns "fileName.ext"(with double quotes) in beta 3

            var dataAcces = new data_access.ImagesMongo();
            dataAcces.Save(file.OpenReadStream(), fileName);
            return Guid.NewGuid();
        }
    }
}