using System.Collections.Generic;
using artmdv_webapi.Models;
using Microsoft.AspNet.Cors;
using Microsoft.AspNet.Mvc;

namespace artmdv_webapi.Controllers
{
    [Route("[controller]")]
    [EnableCors("default")]
    public class AstrophotographyController : Controller
    {
        // GET: values
        [HttpGet]
        public IEnumerable<AstroImage> Get()
        {
            var list = new List<AstroImage>();
            list.Add(new AstroImage() { Image = "images/ap/NGC6995.jpg", Thumbnail = "images/ap/NGC6995_thumb.jpg", Title = "NGC 6995 Eastern Veil nebula" });
            list.Add(new AstroImage() { Image = "images/ap/M16.jpg", Thumbnail = "images/ap/M16_thumb.jpg", Title = "M16 Eagle nebula" });
            list.Add(new AstroImage() { Image = "images/ap/M51.jpg", Thumbnail = "images/ap/M51_thumb.jpg", Title = "M51 Whirlpool galaxy" });
            list.Add(new AstroImage() { Image = "images/ap/M57.jpg", Thumbnail = "images/ap/M57_thumb.jpg", Title = "M57 Ring nebula" });
            list.Add(new AstroImage() { Image = "images/ap/M81_M82.jpg", Thumbnail = "images/ap/M81_M82_thumb.jpg", Title = "M81 Bode's galaxy and M82 Cigar galaxy" });
            list.Add(new AstroImage() { Image = "images/ap/milky-way.jpg", Thumbnail = "images/ap/milky-way_thumb.jpg", Title = "Milky Way" });
            list.Add(new AstroImage() { Image = "images/ap/NGC7331.jpg", Thumbnail = "images/ap/NGC7331_thumb.jpg", Title = "NGC 7331 and friends" });
            return list;
        }


//        // GET values/5
//        [HttpGet("{id}")]
//        public AstroImage Get(int id)
//        {
//            return null;
//        }

//        // POST values
//        [HttpPost]
//        public void Post([FromBody]string value)
//        {
//        }
//
//        // PUT values/5
//        [HttpPut("{id}")]
//        public void Put(int id, [FromBody]string value)
//        {
//        }
//
//        // DELETE values/5
//        [HttpDelete("{id}")]
//        public void Delete(int id)
//        {
//        }
    }
}
