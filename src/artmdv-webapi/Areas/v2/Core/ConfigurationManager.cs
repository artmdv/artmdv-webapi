using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace artmdv_webapi.Areas.v2.Core
{
    public class ConfigurationManager
    {
        public static string GetValue(string key)
        {
            var fs = new FileStream("config.json", FileMode.Open, FileAccess.Read);
            JObject config = null;
            using (StreamReader streamReader = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                config = (JObject)JToken.ReadFrom(reader);
            }
            return config?.GetValue(key).ToString();
        }

        public static string GetPassword()
        {
            return GetValue("password");
        }
    }
}
