using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NEVOLAplus.Intranet.Data;
using NEVOLAplus.Intranet.Models.Inventory;

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

        private bool AssetTypeExists(int id)
        {
            return _context.AssetTypes.Any(e => e.AssetTypeId == id);
        }
    }
}
