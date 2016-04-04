using Crayon.Api.Sdk;
using Crayon.Api.Sdk.Domain.Organizations;
using MultiUserApplication.Models;
using System.Web.Mvc;

namespace MultiUserApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly CrayonApiClient _client;

        public HomeController(CrayonApiClient client, AppSettings appSettings)
        {
            _client = client;
            _appSettings = appSettings;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Organizations()
        {
            ViewBag.Message = "Organizations:";

            if (Session["username"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string token = _client.Tokens.GetUserToken(_appSettings.CrayonClientId(), _appSettings.CrayonClientSecret(), Session["username"] as string, Session["password"] as string).GetData().AccessToken;
            OrganizationCollection organizations = _client.Organizations.Get(token).GetData();
            return View(organizations);
        }
    }
}