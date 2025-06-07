using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NEVOLAplus.Data;
using NEVOLAplus.Data.Models.CMS;

namespace NEVOLAplus.Intranet.Controllers
{
    public class TextSnippetController : Controller
    {
        private readonly NevolaIntranetContext _context;
        public TextSnippetController(NevolaIntranetContext context) => _context = context;

        // GET: TextSnippet
        public async Task<IActionResult> Index()
        {
            var list = await _context.TextSnippets
                                     .OrderBy(s => s.Key)
                                     .ToListAsync();
            return View(list);
        }

        // GET: TextSnippet/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var snip = await _context.TextSnippets.FindAsync(id);
            if (snip == null) return NotFound();
            return View(snip);
        }

        // GET: TextSnippet/Create
        public IActionResult Create() => View();

        // POST: TextSnippet/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Key,Content")] TextSnippet snippet)
        {
            if (!ModelState.IsValid) return View(snippet);
            _context.Add(snippet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: TextSnippet/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var snip = await _context.TextSnippets.FindAsync(id);
            if (snip == null) return NotFound();
            return View(snip);
        }

        // POST: TextSnippet/Edit/5
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TextSnippetId,Key,Content")] TextSnippet snippet)
        {
            if (id != snippet.TextSnippetId) return NotFound();
            if (!ModelState.IsValid) return View(snippet);
            _context.Update(snippet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: TextSnippet/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var snip = await _context.TextSnippets.FindAsync(id);
            if (snip == null) return NotFound();
            return View(snip);
        }

        // POST: TextSnippet/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var snip = await _context.TextSnippets.FindAsync(id);
            if (snip != null) _context.TextSnippets.Remove(snip);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
