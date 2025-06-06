using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NEVOLAplus.Data;                    // kontekst EntityFramework z projektu NEVOLAplus.Data
using NEVOLAplus.Portal.Services;         // interfejs ITextSnippetService

namespace NEVOLAplus.Portal.Services
{
    /// <summary>
    /// Implementacja ITextSnippetService, która wstrzykuje NevolaIntranetContext i pobiera snippet po kluczu.
    /// </summary>
    public class TextSnippetService : ITextSnippetService
    {
        private readonly NevolaIntranetContext _context;

        public TextSnippetService(NevolaIntranetContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<string?> GetContentByKeyAsync(string key)
        {
            // Asynchroniczne jednolinijkowe zapytanie: SELECT Content FROM TextSnippets WHERE Key = @key
            return await _context.TextSnippets
                                 .Where(s => s.Key == key)
                                 .Select(s => s.Content)
                                 .FirstOrDefaultAsync();
        }
    }
}
