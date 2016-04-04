using System.Web.Mvc;
using Crayon.Api.Sdk;
using Crayon.Api.Sdk.Domain.Organizations;
using SingleUserApplication.Handlers;
using SingleUserApplication.Models;

namespace SingleUserApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly CrayonApiClient _client;
        private readonly AppSettings _settings;
        private readonly TokenHandler _tokenHandler;

        public HomeController(CrayonApiClient client, AppSettings settings, TokenHandler tokenHandler)
        {
            _client = client;
            _settings = settings;
            _tokenHandler = tokenHandler;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Organizations()
        {
            if (!_settings.Valid())
            {
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Organizations:";

            string token = _tokenHandler.GetUserToken();
            OrganizationCollection organizations = _client.Organizations.Get(token).GetData();
            return View(organizations);
        }
    }
}