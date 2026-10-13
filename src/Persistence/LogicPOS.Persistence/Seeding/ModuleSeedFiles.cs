using LogicPOS.Persistence.Database;

namespace LogicPOS.Persistence.Seeding;

/// <summary>
/// Resolves module seed JSON paths with fallback to <c>default</c> so mandatory
/// auxiliary tables (customer types, price types, …) still load when a vertical
/// module folder is incomplete or the module token is aliased.
/// </summary>
internal static class ModuleSeedFiles
{
    private static readonly Dictionary<string, string> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["peixaria"] = "seafoodstore",
        ["seafoodshop"] = "seafoodstore",
        ["seafood"] = "seafoodstore",
        ["talho"] = "butchery",
        ["padaria"] = "bakery",
        ["cafe"] = "cafe",
        ["café"] = "cafe"
    };

    internal static string NormalizeModule(string? module)
    {
        var token = string.IsNullOrWhiteSpace(module) ? "default" : module.Trim().ToLowerInvariant();
        return Aliases.TryGetValue(token, out var mapped) ? mapped : token;
    }

    internal static string Resolve(DatabaseSettings settings, string fileName)
    {
        var seedRoot = settings.SeedPath ?? "db_seeds";
        var module = NormalizeModule(settings.Module);
        var preferred = Path.Combine(seedRoot, "modules", module, fileName);
        if (File.Exists(preferred))
        {
            return preferred;
        }

        var fallback = Path.Combine(seedRoot, "modules", "default", fileName);
        return fallback;
    }
}
