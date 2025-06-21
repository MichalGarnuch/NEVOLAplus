using Microsoft.AspNetCore.Mvc;

namespace NEVOLAplus.Intranet.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}