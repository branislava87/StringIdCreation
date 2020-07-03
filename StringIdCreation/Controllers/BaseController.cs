using System.Configuration;
using System.Web.Mvc;

namespace StringIdCreation.Controllers
{
    public class BaseController : Controller
    {

        public string GetUrl(string enviroment)
        {
            switch (enviroment)
            {
                case "DEV":
                    return GetAppSetting("DevEnv");
                case "TEST":
                    return GetAppSetting("TestEnv");
                case "PROD":
                    return GetAppSetting("ProdEnv");
                case "LLT":
                    return GetAppSetting("LltEnv");
                default:
                    return null;
            }
        }

        private string GetAppSetting(string key) => ConfigurationManager.AppSettings[key];
    }
}