using Crayon.Api.Sdk;
using MultiUserApplication.Models;
using System.Net;
using System.Web.Mvc;

namespace MultiUserApplication.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppSettings _appSettings;
        private readonly CrayonApiClient _client;

        public AccountController(CrayonApiClient client, AppSettings appSettings)
        {
            _client = client;
            _appSettings = appSettings;
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _client.Tokens.GetUserToken(_appSettings.CrayonClientId(), _appSettings.CrayonClientSecret(), model.Email, model.Password);
            if (result.StatusCode == HttpStatusCode.OK && !string.IsNullOrEmpty(result.Data?.AccessToken))
            {
                Session["username"] = model.Email;
                Session["password"] = model.Password;
                return RedirectToLocal(returnUrl);
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        public ActionResult LogOff()
        {
            Session["username"] = null;
            Session["password"] = null;
            return RedirectToAction("Index", "Home");
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}