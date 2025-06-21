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
            // Tu w przyszłości można dodać obsługę uwierzytelniania
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            // Po wylogowaniu wyświetlamy stronę logowania
            return RedirectToAction("Login");
        }
    }
}