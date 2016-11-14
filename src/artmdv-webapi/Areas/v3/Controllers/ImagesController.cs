using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace artmdv_webapi.Areas.v3.Controllers
{
    [Area("v3")]
    [Route("[area]/Images")]
    [EnableCors("default")]
    public class ImagesController : Controller
    {
        public ImagesController()
        {
        }
    }
}
