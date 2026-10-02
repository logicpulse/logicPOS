using System.Globalization;
using System.Resources;

namespace LogicPOS.App;

internal static class PreferenceLabels
{
    // Manifest name follows the Link path of the Resx*.resx items in logicpos.csproj
    private static readonly ResourceManager Resources = new("LogicPOS.App.Localization.Resx", typeof(PreferenceLabels).Assembly);

    public static string Get(string? resourceKey, string? token)
    {
        var key = string.IsNullOrWhiteSpace(resourceKey) ? token : resourceKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            return string.Empty;
        }

        var text = Lookup(key);
        if (text is not null)
        {
            return Title(text);
        }

        return Title(key.StartsWith("prefparam_", StringComparison.OrdinalIgnoreCase) && string.IsNullOrWhiteSpace(token) == false
            ? token
            : key);
    }

    public static string Text(string key, string fallback) => Lookup(key) is { } text ? Title(text) : fallback;

    public static string Title(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var split = text.IndexOf("::", StringComparison.Ordinal);
        if (split < 0)
        {
            return text;
        }

        var title = text[(split + 2)..].Trim();
        return string.IsNullOrWhiteSpace(title) ? text : title;
    }

    private static string? Lookup(string key)
    {
        try
        {
            var text = Resources.GetString(key, CultureInfo.CurrentUICulture);
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }
        catch (MissingManifestResourceException)
        {
            return null;
        }
    }
}
