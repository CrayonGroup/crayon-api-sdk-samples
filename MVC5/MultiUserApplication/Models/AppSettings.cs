using System;
using System.Configuration;

namespace MultiUserApplication.Models
{
    public class AppSettings
    {
        public string CrayonClientId()
        {
            string clientId = ConfigurationManager.AppSettings["CrayonClientId"];
            if (string.IsNullOrEmpty(clientId))
            {
                throw new Exception("Please add your crayon clientId to appSettings in web.config.");
            }

            return clientId;
        }

        public string CrayonClientSecret()
        {
            string clientSecret = ConfigurationManager.AppSettings["CrayonClientSecret"];
            if (string.IsNullOrEmpty(clientSecret))
            {
                throw new Exception("Please add your crayon clientSecret to appSettings in web.config.");
            }

            return ConfigurationManager.AppSettings["CrayonClientSecret"];
        }
    }
}