namespace LogicPOS.Domain.ValueObjects;

/// <summary>
/// País que determina o regime fiscal do sistema (AT, AGT, SAF-T). Não usar para moradas.
/// </summary>
public sealed record FiscalCountry
{
    public static readonly FiscalCountry Portugal = new("PT");
    public static readonly FiscalCountry Angola = new("AO");
    public static readonly FiscalCountry Mozambique = new("MZ");

    private FiscalCountry(string code2)
    {
        Code2 = code2;
    }

    public string Code2 { get; }

    public bool IsPortugal => Equals(Portugal);
    public bool IsAngola => Equals(Angola);
    public bool IsMozambique => Equals(Mozambique);

    public static bool TryParse(string? code2, out FiscalCountry country)
    {
        switch (code2?.Trim().ToUpperInvariant())
        {
            case "PT":
                country = Portugal;
                return true;
            case "AO":
                country = Angola;
                return true;
            case "MZ":
                country = Mozambique;
                return true;
            default:
                country = Portugal;
                return false;
        }
    }

    public static FiscalCountry Parse(string? code2)
    {
        if (TryParse(code2, out FiscalCountry country))
            return country;

        throw new ArgumentException(
            $"País fiscal não suportado: '{code2}'. Valores válidos: PT, AO, MZ.", nameof(code2));
    }

    public override string ToString() => Code2;
}
