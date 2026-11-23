namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Maps known domain enum names to PT-PT labels for BackOffice grids and editors.
/// </summary>
public static class ListingEnumDisplay
{
    private static readonly Dictionary<string, string> Labels = new(StringComparer.Ordinal)
    {
        ["Free"] = "Livre",
        ["Open"] = "Aberto",
        ["Reserved"] = "Reservada",
        ["Closed"] = "Fechado",
        ["Day"] = "Dia",
        ["Terminal"] = "Terminal",
        ["None"] = "Nenhum"
    };

    public static string Label(string? enumName)
    {
        if (string.IsNullOrWhiteSpace(enumName))
        {
            return string.Empty;
        }

        return Labels.TryGetValue(enumName, out var label) ? label : enumName;
    }

    public static IReadOnlyList<ListingChoice> Choices(Type enumType)
        => Enum.GetNames(enumType)
            .Select(name => new ListingChoice(name, Label(name)))
            .ToList();
}
