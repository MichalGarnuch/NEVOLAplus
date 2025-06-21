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
    public class AssetController : Controller
    {
        private readonly NevolaIntranetContext _context;

        public AssetController(NevolaIntranetContext context)
        {
            _context = context;
        }

        // GET: Asset Asset
        public async Task<IActionResult> Index()
        {
            var nevolaIntranetContext = _context.Assets.Include(a => a.AssetType);
            return View(await nevolaIntranetContext.ToListAsync());
        }

        // GET: Asset/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .Include(a => a.AssetType)
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // GET: Asset/Create
        public IActionResult Create()
        {
            PopulateAssetTypes();
            return View();
        }

        // POST: Asset/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AssetId,Name,PurchaseDate,Cost,AssetTypeId")] Asset asset)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asset);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateAssetTypes(asset.AssetTypeId);
            return View(asset);
        }

        // GET: Asset/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return NotFound();
            PopulateAssetTypes(asset.AssetTypeId);
            return View(asset);
        }

        // POST: Asset/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssetId,Name,PurchaseDate,Cost,AssetTypeId")] Asset asset)
        {
            if (id != asset.AssetId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asset);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Assets.Any(e => e.AssetId == asset.AssetId))
                        return NotFound();
                    throw;
                }
            }
            PopulateAssetTypes(asset.AssetTypeId);
            return View(asset);
        }
        private void PopulateAssetTypes(int? selectedId = null)
        {
            ViewData["AssetTypeId"] = _context.AssetTypes
                .Select(t => new SelectListItem
                {
                    Value = t.AssetTypeId.ToString(),
                    Text = t.Name,
                    Selected = (t.AssetTypeId == selectedId)
                })
                .ToList();
        }

        // GET: Asset/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asset = await _context.Assets
                .Include(a => a.AssetType)
                .FirstOrDefaultAsync(m => m.AssetId == id);
            if (asset == null)
            {
                return NotFound();
            }

            return View(asset);
        }

        // POST: Asset/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
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

            var assets = await _context.Assets
                .Where(a => selectedIds.Contains(a.AssetId))
                .ToListAsync();

            _context.Assets.RemoveRange(assets);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            var assets = await _context.Assets
                .Include(a => a.AssetType)
                .ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Assets");

            ws.Cells[1, 1].Value = "AssetId";
            ws.Cells[1, 2].Value = "Name";
            ws.Cells[1, 3].Value = "PurchaseDate";
            ws.Cells[1, 4].Value = "Cost";
            ws.Cells[1, 5].Value = "AssetTypeId";

            for (int i = 0; i < assets.Count; i++)
            {
                var a = assets[i];
                ws.Cells[i + 2, 1].Value = a.AssetId;
                ws.Cells[i + 2, 2].Value = a.Name;
                ws.Cells[i + 2, 3].Value = a.PurchaseDate.ToString("yyyy-MM-dd");
                ws.Cells[i + 2, 4].Value = a.Cost;
                ws.Cells[i + 2, 5].Value = a.AssetTypeId;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            var fileName = $"Assets-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
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
                var purchaseDateText = ws.Cells[row, 3].Text;
                var costText = ws.Cells[row, 4].Text;
                var typeText = ws.Cells[row, 5].Text; // tu może być NAZWA typu

                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(typeText))
                    continue;

                DateTime.TryParse(purchaseDateText, out var purchaseDate);
                decimal.TryParse(costText, out var cost);

                var type = await _context.AssetTypes.FirstOrDefaultAsync(t => t.Name == typeText);
                if (type == null)
                {
                    type = new AssetType { Name = typeText };
                    _context.AssetTypes.Add(type);
                    await _context.SaveChangesAsync();
                }

                var asset = new Asset
                {
                    Name = name,
                    PurchaseDate = purchaseDate == default ? DateTime.Now : purchaseDate,
                    Cost = cost,
                    AssetTypeId = type.AssetTypeId
                };
                _context.Assets.Add(asset);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.AssetId == id);
        }
    }
}
