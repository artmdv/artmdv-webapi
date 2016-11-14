using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Configuration
{
    public class ConfigReader
    {
        public static string RabbitConnectionString => Get("rabbitConnectionString");
        public static string RabbitSubscriptionId => Get("rabbitSubscriptionId");
        public static string EventStoreAddress => Get("eventStoreAddress");
        public static string EventStoreUser => Get("eventStoreUser");
        public static string EventStorePassword => Get("eventStorePassword");

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
