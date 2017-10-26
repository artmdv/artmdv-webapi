using System.IO;
using artmdv_webapi.Areas.v2.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace artmdv_webapi.Areas.v2.Core
{
    public class ConfigurationManager: IConfigurationManager
    {
        public IFile File { get; }

        public ConfigurationManager(IFile file)
        {
            File = file;
        }

        public string GetValue(string key)
        {
            var fs = File.Open("config.json");
            JObject config = null;
            using (StreamReader streamReader = new StreamReader(fs))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                config = (JObject)JToken.ReadFrom(reader);
            }
            return config?.GetValue(key).ToString();
        }

        public string GetPassword()
        {
            return GetValue("password");
        }
    }

    public interface IConfigurationManager
    {
        string GetValue(string key);
        string GetPassword();
    }
}
