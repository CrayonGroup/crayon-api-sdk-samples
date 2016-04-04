using System.Configuration;

namespace SingleUserApplication.Models
{
    public class AppSettings
    {
        public bool Valid()
        {
            return !string.IsNullOrEmpty(CrayonClientId()) &&
                   !string.IsNullOrEmpty(CrayonClientSecret()) &&
                   !string.IsNullOrEmpty(CrayonUserName()) &&
                   !string.IsNullOrEmpty(CrayonUserPassword());
        }

        public string CrayonClientId()
        {
            return ConfigurationManager.AppSettings["CrayonClientId"];
        }

        public string CrayonClientSecret()
        {
            return ConfigurationManager.AppSettings["CrayonClientSecret"];
        }

        public string CrayonUserName()
        {
            return ConfigurationManager.AppSettings["CrayonUserName"];
        }

        public string CrayonUserPassword()
        {
            return ConfigurationManager.AppSettings["CrayonUserPassword"];
        }
    }
}