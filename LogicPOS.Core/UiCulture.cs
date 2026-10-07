using System.Globalization;

namespace LogicPOS.Core;

public static class UiCulture
{
    public static void Apply(string name)
    {
        var culture = CultureInfo.GetCultureInfo(name);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        var path = Path.Combine(AppContext.BaseDirectory, "ui-culture.txt");
        File.WriteAllText(path, culture.Name);
    }

    public static string Read(string? fallback)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "ui-culture.txt");
        if (File.Exists(path))
        {
            var stored = File.ReadAllText(path).Trim();
            if (stored.Length > 0)
            {
                return stored;
            }
        }

        return string.IsNullOrWhiteSpace(fallback) ? "pt-PT" : fallback;
    }
}
