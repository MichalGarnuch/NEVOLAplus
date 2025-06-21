using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using NEVOLAplus.Data;
using NEVOLAplus.Data.Models.Inventory;

namespace NEVOLAplus.Intranet.Controllers
{
    public class AssetTypeController : Controller
    {
        private readonly NevolaIntranetContext _context;

        public AssetTypeController(NevolaIntranetContext context)
        {
            _context = context;
        }

        // GET: AssetType
        public async Task<IActionResult> Index()
        {
            return View(await _context.AssetTypes.ToListAsync());
        }

        // GET: AssetType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetType = await _context.AssetTypes
                .FirstOrDefaultAsync(m => m.AssetTypeId == id);
            if (assetType == null)
            {
                return NotFound();
            }

            return View(assetType);
        }

        // GET: AssetType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AssetType/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssetTypeId,Name,Description")] AssetType assetType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(assetType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(assetType);
        }

        // GET: AssetType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetType = await _context.AssetTypes.FindAsync(id);
            if (assetType == null)
            {
                return NotFound();
            }
            return View(assetType);
        }

        // POST: AssetType/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssetTypeId,Name,Description")] AssetType assetType)
        {
            if (id != assetType.AssetTypeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assetType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssetTypeExists(assetType.AssetTypeId))
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
            return View(assetType);
        }

        // GET: AssetType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assetType = await _context.AssetTypes
                .FirstOrDefaultAsync(m => m.AssetTypeId == id);
            if (assetType == null)
            {
                return NotFound();
            }

            return View(assetType);
        }

        // POST: AssetType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assetType = await _context.AssetTypes.FindAsync(id);
            if (assetType != null)
            {
                _context.AssetTypes.Remove(assetType);
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

            var types = await _context.AssetTypes
                .Where(t => selectedIds.Contains(t.AssetTypeId))
                .ToListAsync();

            _context.AssetTypes.RemoveRange(types);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            var types = await _context.AssetTypes.ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("AssetTypes");

            ws.Cells[1, 1].Value = "AssetTypeId";
            ws.Cells[1, 2].Value = "Name";
            ws.Cells[1, 3].Value = "Description";

            for (int i = 0; i < types.Count; i++)
            {
                var t = types[i];
                ws.Cells[i + 2, 1].Value = t.AssetTypeId;
                ws.Cells[i + 2, 2].Value = t.Name;
                ws.Cells[i + 2, 3].Value = t.Description;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            var fileName = $"AssetTypes-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
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
                var name = ws.Cells[row, 2].Text;
                var desc = ws.Cells[row, 3].Text;

                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var type = new AssetType
                {
                    Name = name,
                    Description = desc ?? string.Empty
                };
                _context.AssetTypes.Add(type);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool AssetTypeExists(int id)
        {
            return _context.AssetTypes.Any(e => e.AssetTypeId == id);
        }
    }
}
