using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace NEVOLAplus.Intranet.Security;

// Explicit single-operator mode. A multi-user deployment should use an IdP/Identity store.
public sealed class AdminAccess
{
    private readonly PasswordHasher<AdminAccess> hasher = new();
    public string Username { get; }
    private string PasswordHash { get; }
    public string Version { get; }
    public AdminAccess(IConfiguration config)
    {
        Username = config["Auth:Username"]?.Trim() ?? "";
        PasswordHash = config["Auth:PasswordHash"] ?? "";
        if (Username.Length is < 1 or > 128 || PasswordHash.Length is < 40 or > 512)
            throw new InvalidOperationException("Configure Auth:Username and Auth:PasswordHash (ASP.NET Identity format).");
        Version = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Username + ":" + PasswordHash)));
    }
    public bool Verify(string username, string password)
    {
        if (password.Length > 256 || username.Length > 128) return false;
        // Always run the password verifier so an invalid username has a similar path.
        PasswordVerificationResult result;
        try { result = hasher.VerifyHashedPassword(this, PasswordHash, password); }
        catch (FormatException) { return false; }
        return string.Equals(Username, username, StringComparison.Ordinal) && result != PasswordVerificationResult.Failed;
    }
}
