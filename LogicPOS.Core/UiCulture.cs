using System.Globalization;

namespace LogicPOS.Core;

public static class UiCulture
{
    private static readonly object Gate = new();
    private static string? _cached;

    public static void Apply(string name)
    {
        var culture = Resolve(name) ?? CultureInfo.GetCultureInfo("pt-PT");

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "ui-culture.txt");
            File.WriteAllText(path, culture.Name);
        }
        catch
        {
            // Disk full / read-only — keep in-memory culture anyway.
        }

        lock (Gate)
        {
            _cached = culture.Name;
        }
    }

    public static string Read(string? fallback)
    {
        var def = string.IsNullOrWhiteSpace(fallback) ? "pt-PT" : fallback!;
        lock (Gate)
        {
            if (_cached is not null)
            {
                return _cached;
            }
        }

        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "ui-culture.txt");
            if (File.Exists(path))
            {
                var stored = File.ReadAllText(path).Trim();
                var resolved = Resolve(stored);
                if (resolved is not null)
                {
                    lock (Gate)
                    {
                        _cached = resolved.Name;
                    }

                    return resolved.Name;
                }
            }
        }
        catch
        {
            // Invalid culture name or I/O — fall through to default.
        }

        var fallbackCulture = Resolve(def) ?? CultureInfo.GetCultureInfo("pt-PT");
        lock (Gate)
        {
            _cached ??= fallbackCulture.Name;
            return _cached;
        }
    }

    /// <summary>Test helper: set or clear the in-memory culture without touching the file.</summary>
    public static void SetCachedForTests(string? name)
    {
        lock (Gate)
        {
            _cached = name;
        }
    }

    /// <summary>
    /// Accepts only cultures whose resolved <see cref="CultureInfo.Name"/> matches the request
    /// (rejects invented parents such as <c>not-a-real-…</c> → <c>not</c>).
    /// </summary>
    private static CultureInfo? Resolve(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        try
        {
            var culture = CultureInfo.GetCultureInfo(name.Trim());
            if (string.Equals(culture.Name, name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return culture;
            }
        }
        catch (CultureNotFoundException)
        {
        }

        return null;
    }
}
