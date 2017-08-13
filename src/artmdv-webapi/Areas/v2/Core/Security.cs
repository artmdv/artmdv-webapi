using System.Net.Http;

namespace artmdv_webapi.Areas.v2.Core
{
    public class SecurityHandler: ISecurityHandler
    {
        public IConfigurationManager ConfigurationManager { get; }

        public SecurityHandler(IConfigurationManager configurationManager)
        {
            ConfigurationManager = configurationManager;
        }

        public bool IsValidPassword(string password)
        {
            return ConfigurationManager.GetPassword() == password;
        }
    }

    public interface ISecurityHandler
    {
        bool IsValidPassword(string password);
    }
}
