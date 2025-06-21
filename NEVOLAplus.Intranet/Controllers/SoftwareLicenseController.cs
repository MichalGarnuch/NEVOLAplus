using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using NEVOLAplus.Data;
using NEVOLAplus.Data.Models.Licensing;

namespace NEVOLAplus.Intranet.Controllers
{
    public class SoftwareLicenseController : Controller
    {
        private readonly NevolaIntranetContext _context;

        public SoftwareLicenseController(NevolaIntranetContext context)
        {
            _context = context;
        }

        // GET: SoftwareLicense
        public async Task<IActionResult> Index()
        {
            return View(await _context.SoftwareLicenses.ToListAsync());
        }

        // GET: SoftwareLicense/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareLicense = await _context.SoftwareLicenses
                .FirstOrDefaultAsync(m => m.SoftwareLicenseId == id);
            if (softwareLicense == null)
            {
                return NotFound();
            }

            return View(softwareLicense);
        }

        // GET: SoftwareLicense/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SoftwareLicense/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SoftwareLicenseId,ProductName,LicenseKey,ValidUntil")] SoftwareLicense softwareLicense)
        {
            if (ModelState.IsValid)
            {
                _context.Add(softwareLicense);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(softwareLicense);
        }

        // GET: SoftwareLicense/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareLicense = await _context.SoftwareLicenses.FindAsync(id);
            if (softwareLicense == null)
            {
                return NotFound();
            }
            return View(softwareLicense);
        }

        // POST: SoftwareLicense/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SoftwareLicenseId,ProductName,LicenseKey,ValidUntil")] SoftwareLicense softwareLicense)
        {
            if (id != softwareLicense.SoftwareLicenseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(softwareLicense);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoftwareLicenseExists(softwareLicense.SoftwareLicenseId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(softwareLicense);
        }

        // GET: SoftwareLicense/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareLicense = await _context.SoftwareLicenses
                .FirstOrDefaultAsync(m => m.SoftwareLicenseId == id);
            if (softwareLicense == null)
            {
                return NotFound();
            }

            return View(softwareLicense);
        }

        // POST: SoftwareLicense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var softwareLicense = await _context.SoftwareLicenses.FindAsync(id);
            if (softwareLicense != null)
            {
                _context.SoftwareLicenses.Remove(softwareLicense);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelected(int[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length == 0)
                return RedirectToAction(nameof(Index));

            var licenses = await _context.SoftwareLicenses
                .Where(l => selectedIds.Contains(l.SoftwareLicenseId))
                .ToListAsync();

            _context.SoftwareLicenses.RemoveRange(licenses);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Export()
        {
            var licenses = await _context.SoftwareLicenses.ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("SoftwareLicenses");

            ws.Cells[1, 1].Value = "SoftwareLicenseId";
            ws.Cells[1, 2].Value = "ProductName";
            ws.Cells[1, 3].Value = "LicenseKey";
            ws.Cells[1, 4].Value = "ValidUntil";

            for (int i = 0; i < licenses.Count; i++)
            {
                var l = licenses[i];
                ws.Cells[i + 2, 1].Value = l.SoftwareLicenseId;
                ws.Cells[i + 2, 2].Value = l.ProductName;
                ws.Cells[i + 2, 3].Value = l.LicenseKey;
                ws.Cells[i + 2, 4].Value = l.ValidUntil.ToString("yyyy-MM-dd");
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            var fileName = $"SoftwareLicenses-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return RedirectToAction(nameof(Index));

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(stream);
            var ws = package.Workbook.Worksheets.First();
            var rows = ws.Dimension.Rows;

            for (int row = 2; row <= rows; row++)
            {
                var product = ws.Cells[row, 2].Text;
                var key = ws.Cells[row, 3].Text;
                var validText = ws.Cells[row, 4].Text;

                if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(key))
                    continue;

                DateTime.TryParse(validText, out var valid);

                var license = new SoftwareLicense
                {
                    ProductName = product,
                    LicenseKey = key,
                    ValidUntil = valid == default ? DateTime.Now : valid
                };
                _context.SoftwareLicenses.Add(license);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool SoftwareLicenseExists(int id)
        {
            return _context.SoftwareLicenses.Any(e => e.SoftwareLicenseId == id);
        }
    }
}
