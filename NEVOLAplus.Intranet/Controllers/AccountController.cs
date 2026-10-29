using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using NEVOLAplus.Intranet.Security;

namespace NEVOLAplus.Intranet.Controllers;
public class AccountController : Controller
{
    private readonly AdminAccess access;
    public AccountController(AdminAccess access) { this.access = access; }
    [AllowAnonymous, HttpGet]
    public IActionResult Login() => View();
    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken, EnableRateLimiting("login")]
    public async Task<IActionResult> Login(string? username, string? password)
    {
        if (!access.Verify(username ?? "", password ?? ""))
        {
            ModelState.AddModelError("", "Nieprawidłowe dane logowania.");
            return View();
        }
        var identity = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.Name, access.Username), new Claim(ClaimTypes.Role, "Administrator"),
            new Claim("credential_version", access.Version),
        }, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        return RedirectToAction("Index", "Home");
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
