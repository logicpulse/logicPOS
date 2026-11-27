using LogicPOS.Core;

namespace LogicPOS.App.Views;

/// <summary>
/// Operation-mode captions for the first entry. Same names as the web first entry
/// (and the installer, for German). Follows <see cref="UiCulture"/>, not the OS language.
/// </summary>
internal static class FirstEntryModeText
{
    public static string Label(string mode)
    {
        var token = mode.Trim().ToLowerInvariant();
        var culture = UiCulture.Read(null);
        var table = culture.StartsWith("de", StringComparison.OrdinalIgnoreCase) ? German
            : culture.StartsWith("es", StringComparison.OrdinalIgnoreCase) ? Spanish
            : culture.StartsWith("fr", StringComparison.OrdinalIgnoreCase) ? French
            : culture.StartsWith("en", StringComparison.OrdinalIgnoreCase) ? English
            : Portuguese;
        return table.TryGetValue(token, out var label) ? label : mode;
    }

    private static readonly Dictionary<string, string> Portuguese = new(StringComparer.OrdinalIgnoreCase)
    {
        ["parking"] = "Parking",
        ["bakery"] = "Padaria",
        ["butchery"] = "Talho",
        ["cafe"] = "Café / Bar",
        ["clothingstore"] = "Loja de Roupa",
        ["hardwarestore"] = "Ferragens / Drogaria",
        ["restaurant"] = "Restauração",
        ["seafoodstore"] = "Peixaria",
        ["shoestore"] = "Sapataria"
    };

    private static readonly Dictionary<string, string> English = new(StringComparer.OrdinalIgnoreCase)
    {
        ["parking"] = "Parking",
        ["bakery"] = "Bakery",
        ["butchery"] = "Butchery",
        ["cafe"] = "Café / Bar",
        ["clothingstore"] = "Clothing store",
        ["hardwarestore"] = "Hardware / drugstore",
        ["restaurant"] = "Restaurant",
        ["seafoodstore"] = "Seafood",
        ["shoestore"] = "Shoe store"
    };

    private static readonly Dictionary<string, string> German = new(StringComparer.OrdinalIgnoreCase)
    {
        ["parking"] = "Parkplatz",
        ["bakery"] = "Bäckerei",
        ["butchery"] = "Metzgerei",
        ["cafe"] = "Café / Bar",
        ["clothingstore"] = "Bekleidungsgeschäft",
        ["hardwarestore"] = "Eisenwaren / Drogerie",
        ["restaurant"] = "Restaurant",
        ["seafoodstore"] = "Fischhandlung",
        ["shoestore"] = "Schuhgeschäft"
    };

    private static readonly Dictionary<string, string> Spanish = new(StringComparer.OrdinalIgnoreCase)
    {
        ["parking"] = "Aparcamiento",
        ["bakery"] = "Panadería",
        ["butchery"] = "Carnicería",
        ["cafe"] = "Café / Bar",
        ["clothingstore"] = "Tienda de ropa",
        ["hardwarestore"] = "Ferretería / droguería",
        ["restaurant"] = "Restauración",
        ["seafoodstore"] = "Pescadería",
        ["shoestore"] = "Zapatería"
    };

    private static readonly Dictionary<string, string> French = new(StringComparer.OrdinalIgnoreCase)
    {
        ["parking"] = "Parking",
        ["bakery"] = "Boulangerie",
        ["butchery"] = "Boucherie",
        ["cafe"] = "Café / Bar",
        ["clothingstore"] = "Magasin de vêtements",
        ["hardwarestore"] = "Quincaillerie / droguerie",
        ["restaurant"] = "Restaurant",
        ["seafoodstore"] = "Poissonnerie",
        ["shoestore"] = "Magasin de chaussures"
    };
}
