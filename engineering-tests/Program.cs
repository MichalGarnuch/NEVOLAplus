using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NEVOLAplus.Intranet.Security;

var hasher = new PasswordHasher<AdminAccess>();
var hash = hasher.HashPassword(null!, "test-only-long-password");
var settings = new Dictionary<string, string?> { ["Auth:Username"]="operator", ["Auth:PasswordHash"]=hash };
AdminAccess Access() => new(new ConfigurationBuilder().AddInMemoryCollection(settings).Build());
var access = Access();
void Check(bool condition) { if (!condition) throw new Exception("Credential contract failed"); }
Check(access.Verify("operator", "test-only-long-password"));
Check(!access.Verify("someone-else", "test-only-long-password"));
Check(!access.Verify("operator", "wrong-password"));
settings["Auth:PasswordHash"] = hasher.HashPassword(null!, "another-test-only-password");
Check(Access().Version != access.Version);
settings["Auth:PasswordHash"] = null;
try { Access(); throw new Exception("Missing configuration accepted"); }
catch (InvalidOperationException) { }
Console.WriteLine("Admin credential contracts passed (5 checks).");
