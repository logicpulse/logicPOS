namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Display values for the BackOffice receipt list. Receipt rows do not store
/// customer or document number on the entity itself.
/// </summary>
public static class ReceiptListFormatting
{
    public static IReadOnlyList<Guid> DocumentIds(string? notes)
    {
        if (string.IsNullOrWhiteSpace(notes))
        {
            return [];
        }

        var ids = new List<Guid>();
        foreach (var part in notes.Split([',', ';', ' ', '|'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (Guid.TryParse(part, out var id) && id != Guid.Empty && ids.Contains(id) == false)
            {
                ids.Add(id);
            }
        }

        return ids;
    }

    public static string StatusLabel(string? status) => status switch
    {
        "N" => "Normal",
        "A" => "Anulado",
        "F" => "Faturado",
        null or "" => string.Empty,
        _ => status
    };

    public static string JoinDistinct(IEnumerable<string?> values)
    {
        var items = values
            .Where(value => string.IsNullOrWhiteSpace(value) == false)
            .Select(value => value!.Trim())
            .Distinct(StringComparer.CurrentCultureIgnoreCase);
        return string.Join(", ", items);
    }
}
