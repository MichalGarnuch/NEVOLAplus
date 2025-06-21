using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSelected(int[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length == 0)
                return RedirectToAction(nameof(Index));

            var snippets = await _context.TextSnippets
                .Where(s => selectedIds.Contains(s.TextSnippetId))
                .ToListAsync();

            _context.TextSnippets.RemoveRange(snippets);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            var snippets = await _context.TextSnippets.OrderBy(s => s.Key).ToListAsync();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("TextSnippets");

            ws.Cells[1, 1].Value = "TextSnippetId";
            ws.Cells[1, 2].Value = "Key";
            ws.Cells[1, 3].Value = "Content";

            for (int i = 0; i < snippets.Count; i++)
            {
                var s = snippets[i];
                ws.Cells[i + 2, 1].Value = s.TextSnippetId;
                ws.Cells[i + 2, 2].Value = s.Key;
                ws.Cells[i + 2, 3].Value = s.Content;
            }

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;
            var fileName = $"TextSnippets-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
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
                var key = ws.Cells[row, 2].Text;
                var content = ws.Cells[row, 3].Text;

                if (string.IsNullOrWhiteSpace(key))
                    continue;

                var snippet = new TextSnippet
                {
                    Key = key,
                    Content = content ?? string.Empty
                };
                _context.TextSnippets.Add(snippet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
