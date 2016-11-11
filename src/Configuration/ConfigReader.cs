using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Configuration
{
    public class ConfigReader
    {
        public const string RabbitHost = "rabbitHost";
        public const string RabbitPassword = "rabbitPassword";
        public const string RabbitUser = "rabbitUser";


        public static string Get(string name)
        {
            var fs = new FileStream("config.json", FileMode.Open, FileAccess.Read);
            JObject config;
            using (StreamReader streamReader = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                config = (JObject)JToken.ReadFrom(reader);
            }

            return config?.GetValue(name).ToString();
        }
    }
}
