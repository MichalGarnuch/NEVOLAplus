using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NEVOLAplus.Data;
using NEVOLAplus.Data.Models.CMS;

namespace NEVOLAplus.Portal.Controllers
{
    public class TextSnippetController : Controller
    {
        private readonly NevolaIntranetContext _context;

        public TextSnippetController(NevolaIntranetContext context)
        {
            _context = context;
        }

        // GET: TextSnippet
        public async Task<IActionResult> Index()
        {
            return View(await _context.TextSnippets.ToListAsync());
        }

        // GET: TextSnippet/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textSnippet = await _context.TextSnippets
                .FirstOrDefaultAsync(m => m.TextSnippetId == id);
            if (textSnippet == null)
            {
                return NotFound();
            }

            return View(textSnippet);
        }

        // GET: TextSnippet/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TextSnippet/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TextSnippetId,Key,Content")] TextSnippet textSnippet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(textSnippet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(textSnippet);
        }

        // GET: TextSnippet/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textSnippet = await _context.TextSnippets.FindAsync(id);
            if (textSnippet == null)
            {
                return NotFound();
            }
            return View(textSnippet);
        }

        // POST: TextSnippet/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TextSnippetId,Key,Content")] TextSnippet textSnippet)
        {
            if (id != textSnippet.TextSnippetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(textSnippet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TextSnippetExists(textSnippet.TextSnippetId))
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
            return View(textSnippet);
        }

        // GET: TextSnippet/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var textSnippet = await _context.TextSnippets
                .FirstOrDefaultAsync(m => m.TextSnippetId == id);
            if (textSnippet == null)
            {
                return NotFound();
            }

            return View(textSnippet);
        }

        // POST: TextSnippet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var textSnippet = await _context.TextSnippets.FindAsync(id);
            if (textSnippet != null)
            {
                _context.TextSnippets.Remove(textSnippet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TextSnippetExists(int id)
        {
            return _context.TextSnippets.Any(e => e.TextSnippetId == id);
        }
    }
}
