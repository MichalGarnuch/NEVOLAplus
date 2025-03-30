using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        private bool SoftwareLicenseExists(int id)
        {
            return _context.SoftwareLicenses.Any(e => e.SoftwareLicenseId == id);
        }
    }
}
