using LogicPOS.Domain.Enums;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Maps known domain enum names to PT-PT labels for BackOffice grids and editors.
/// Gender follows the owning enum type (mesa / sessão / generic).
/// </summary>
public static class ListingEnumDisplay
{
    public static string Label(Enum value) => Label(value.GetType(), value.ToString());

    public static string Label(Type enumType, string? enumName)
    {
        if (string.IsNullOrWhiteSpace(enumName))
        {
            return string.Empty;
        }

        if (enumType == typeof(TableStatus))
        {
            return enumName switch
            {
                nameof(TableStatus.Free) => "Livre",
                nameof(TableStatus.Open) => "Ocupada",
                nameof(TableStatus.Reserved) => "Reservada",
                _ => enumName
            };
        }

        if (enumType == typeof(WorkSessionPeriodStatus))
        {
            return enumName switch
            {
                nameof(WorkSessionPeriodStatus.Open) => "Aberta",
                nameof(WorkSessionPeriodStatus.Closed) => "Fechada",
                _ => enumName
            };
        }

        if (enumType == typeof(WorkSessionPeriodType))
        {
            return enumName switch
            {
                nameof(WorkSessionPeriodType.Day) => "Dia",
                nameof(WorkSessionPeriodType.Terminal) => "Terminal",
                _ => enumName
            };
        }

        return enumName switch
        {
            "None" => "Nenhum",
            "Free" => "Livre",
            "Open" => "Aberto",
            "Reserved" => "Reservada",
            "Closed" => "Fechado",
            "Day" => "Dia",
            "Terminal" => "Terminal",
            _ => enumName
        };
    }

    /// <summary>Legacy name-only lookup kept for callers without the enum type.</summary>
    public static string Label(string? enumName) => Label(typeof(Enum), enumName);

    public static IReadOnlyList<ListingChoice> Choices(Type enumType)
        => Enum.GetNames(enumType)
            .Select(name => new ListingChoice(name, Label(enumType, name)))
            .ToList();
}
