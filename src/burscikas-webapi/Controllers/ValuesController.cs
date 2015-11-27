using System.Collections.Generic;
using Microsoft.AspNet.Mvc;

namespace burscikas_webapi.Controllers
{
//    [Route("[controller]")]
    public class ValuesController : Controller
    {
        // GET: values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" };
        }

        // GET values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpGet]
        public string Test(string test)
        {
            return test;
        }

        // POST values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
