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
