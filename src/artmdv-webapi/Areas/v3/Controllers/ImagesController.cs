using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Configuration;
using Gallery.Contracts.Events;
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

        [Route("Test")]
        [HttpGet]
        public void TestRabbit()
        {
            var @event = new TestEvent {TestProp = "test"};
            Rabbit.Send(@event);
        }
    }
}
