using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace NEVOLAplus.Intranet.Controllers
{
    public class StyleController : Controller
    {
        private readonly IWebHostEnvironment _env;
        public StyleController(IWebHostEnvironment env)
        {
            _env = env;
        }

        private string GetPortalCssPath()
        {
            return Path.Combine(_env.ContentRootPath, "..", "NEVOLAplus.Portal", "wwwroot", "css", "custom.css");
        }

        public IActionResult Edit()
        {
            var path = GetPortalCssPath();
            var css = System.IO.File.Exists(path) ? System.IO.File.ReadAllText(path) : string.Empty;
            ViewBag.CssContent = css;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string cssContent)
        {
            var path = GetPortalCssPath();
            System.IO.File.WriteAllText(path, cssContent ?? string.Empty);
            return RedirectToAction(nameof(Edit));
        }
    }
}