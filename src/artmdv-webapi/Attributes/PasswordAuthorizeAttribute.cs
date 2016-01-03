using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Mvc.Filters;

namespace artmdv_webapi.Attributes
{
   
    public class PasswordAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string cookie = context.HttpContext.Request.Headers.Where(x => x.Key == "Cookie").Select(y => y.Value).FirstOrDefault().FirstOrDefault(z => z.Contains("password"));
            string password = null;
            if (cookie != null && cookie.Contains("password"))
            {
                password = cookie.Substring(cookie.IndexOf("="));
            }
            if (password != "Labas123")
            {
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            }
        }
    }
}
