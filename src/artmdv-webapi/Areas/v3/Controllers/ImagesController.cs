using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace artmdv_webapi.Areas.v3.Controllers
{
    [Area("v3")]
    [Route("[area]/Images")]
    [EnableCors("default")]
    public class ImagesController : Controller
    {
        public ImagesController(IImageService imagesService)
        {
            
        }
    }
}
