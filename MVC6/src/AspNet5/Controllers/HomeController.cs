using System.Linq;
using Crayon.Api.Sdk;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MVC6.Controllers
{
    public class HomeController : Controller
    {
        private readonly CrayonApiClient _client;

        public HomeController(CrayonApiClient client)
        {
            _client = client;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Organizations()
        {
            ViewBag.Message = "Organizations:";

            var organizations = _client.Organizations.Get(User.Claims.FirstOrDefault(f => f.Type == "token")?.Value).GetData();
            return View(organizations);
        }
    }
}