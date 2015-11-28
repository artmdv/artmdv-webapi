using System.Collections.Generic;
using burscikas_webapi.Models;
using Microsoft.AspNet.Mvc;

namespace burscikas_webapi.Controllers
{
    [Route("[controller]")]
    public class AstrophotographyController : Controller
    {
        // GET: values
        [HttpGet]
        public IEnumerable<AstroImage> Get()
        {
            var list = new List<AstroImage>();
            list.Add(new AstroImage() { Image = "NGC6995.jpg", Thumbnail = "NGC6995_thumb.jpg", Title = "NGC 6995 Eastern Veil nebula" });
            list.Add(new AstroImage() { Image = "M16.jpg", Thumbnail = "M16_thumb.jpg", Title = "M16 Eagle nebula" });
            list.Add(new AstroImage() { Image = "M51.jpg", Thumbnail = "M51_thumb.jpg", Title = "M51 Whirlpool galaxy" });
            list.Add(new AstroImage() { Image = "M57.jpg", Thumbnail = "M57_thumb.jpg", Title = "M57 Ring nebula" });
            list.Add(new AstroImage() { Image = "M81_M82.jpg", Thumbnail = "M81_M82_thumb.jpg", Title = "M81 Bode's galaxy and M82 Cigar galaxy" });
            list.Add(new AstroImage() { Image = "milky-way.jpg", Thumbnail = "milky-way_thumb.jpg", Title = "Milky Way" });
            list.Add(new AstroImage() { Image = "NGC7331.jpg", Thumbnail = "NGC7331_thumb.jpg", Title = "NGC 7331 and friends" });
            return null;
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
