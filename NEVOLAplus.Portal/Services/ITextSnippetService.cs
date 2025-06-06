using System.Threading.Tasks;

namespace NEVOLAplus.Portal.Services
{
    /// <summary>
    /// Serwis służący do pobierania tekstów (snippetów) z bazy danych.
    /// </summary>
    public interface ITextSnippetService
    {
        /// <summary>
        /// Zwraca zawartość tekstu dla podanego klucza. Jeśli wiersza nie ma, zwraca null.
        /// </summary>
        /// <param name="key">Klucz tekstu, np. "Menu_Home", "Portal_Welcome" itp.</param>
        /// <returns>ciąg znaków z pola Content lub null</returns>
        Task<string?> GetContentByKeyAsync(string key);
    }
}
