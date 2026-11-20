namespace LogicPOS.Core.Fiscal;

public sealed record CompanyFiscalMarkState(bool RegimeArtigo53, bool VendedorAmbulante)
{
    public static CompanyFiscalMarkState None { get; } = new(false, false);
}

public static class CompanyFiscalMarks
{
    public static CompanyFiscalMarkState Read(IEnumerable<(string Token, string? Value)> parameters)
    {
        var map = parameters.ToDictionary(item => item.Token, item => item.Value, StringComparer.OrdinalIgnoreCase);
        return new CompanyFiscalMarkState(
            IsTrue(map, CompanyFiscalMarkTokens.RegimeArtigo53),
            IsTrue(map, CompanyFiscalMarkTokens.VendedorAmbulante));
    }

    public static CompanyFiscalMarkState Read(IEnumerable<KeyValuePair<string, string?>> parameters)
        => Read(parameters.Select(item => (item.Key, item.Value)));

    private static bool IsTrue(IReadOnlyDictionary<string, string?> map, string token)
    {
        if (map.TryGetValue(token, out var value) == false)
        {
            return false;
        }

        return string.Equals(value?.Trim(), "true", StringComparison.OrdinalIgnoreCase);
    }
}