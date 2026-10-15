namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Portuguese ATCUD: AT series validation code + sequential document number (GTK ProcessFinanceDocument).
/// </summary>
public static class AtcudFormat
{
    public static string Build(string? seriesValidationCode, string? documentNumber)
    {
        var sequential = SequentialFromNumber(documentNumber);
        if (string.IsNullOrWhiteSpace(sequential))
        {
            return string.Empty;
        }

        var validation = seriesValidationCode?.Trim() ?? string.Empty;
        return $"{validation}-{sequential}";
    }

    public static string? OrFallback(string? storedAtcud, string? seriesValidationCode, string? documentNumber)
    {
        if (string.IsNullOrWhiteSpace(storedAtcud) == false)
        {
            return storedAtcud.Trim();
        }

        var built = Build(seriesValidationCode, documentNumber);
        return string.IsNullOrWhiteSpace(built) ? null : built;
    }

    /// <summary>
    /// Whether the ATCUD text on a document should appear on tickets/PDFs.
    /// Requires a non-empty validation prefix before the last '-' (excluding "0")
    /// and a non-empty sequential after it.
    /// </summary>
    public static bool ShouldPrint(string? atcud)
    {
        if (string.IsNullOrWhiteSpace(atcud))
        {
            return false;
        }

        var value = atcud.Trim();
        var dash = value.LastIndexOf('-');
        if (dash < 0)
        {
            return false;
        }

        var prefix = value[..dash].Trim();
        var sequential = value[(dash + 1)..].Trim();
        return string.IsNullOrWhiteSpace(prefix) == false
               && prefix.Equals("0", StringComparison.Ordinal) == false
               && string.IsNullOrWhiteSpace(sequential) == false;
    }

    private static string SequentialFromNumber(string? documentNumber)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
        {
            return string.Empty;
        }

        var value = documentNumber.Trim().Trim('[', ']');
        var slash = value.LastIndexOf('/');
        return slash >= 0 && slash < value.Length - 1
            ? value[(slash + 1)..].Trim()
            : value;
    }
}
