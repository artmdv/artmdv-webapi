using System.Net.Http;

namespace artmdv_webapi.Areas.v2.Core
{
    public class Security
    {
        public static void ValidatePassword(string password)
        {
            if (ConfigurationManager.GetPassword() != password)
            {
                throw new HttpRequestException();
            }
        }
    }
}
