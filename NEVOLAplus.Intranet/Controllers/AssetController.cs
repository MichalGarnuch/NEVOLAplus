using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        private bool AssetExists(int id)
        {
            return _context.Assets.Any(e => e.AssetId == id);
        }
    }
}
