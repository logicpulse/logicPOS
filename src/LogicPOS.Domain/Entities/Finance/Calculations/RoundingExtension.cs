namespace LogicPOS.Domain.Entities.Finance.Calculations;

public static class RoundingExtension
{
    /// <summary>Max absolute value for SQL <c>decimal(18,6)</c> columns.</summary>
    public const decimal Decimal18_6Max = 999_999_999_999.999999m;

    public static decimal Round(this decimal value) => Math.Round(value, 2);
    public static decimal ApplyAgtRounding(this decimal value) => Math.Ceiling(value * 100) / 100;
    public static decimal TruncateDecimal(this decimal value)
    {
        decimal factor = 1;

        for (int i = 0; i < 2; i++)
        {
            factor *= 10;
        }

        return Math.Truncate(value * factor) / factor;
    }

    /// <summary>
    /// Clamps to <c>decimal(18,6)</c> range and truncates to 6 fractional digits
    /// so EF/SQL Server accept the value (used by legacy import).
    /// </summary>
    public static decimal ClampToDecimal18_6(this decimal value)
    {
        if (value > Decimal18_6Max)
        {
            return Decimal18_6Max;
        }

        if (value < -Decimal18_6Max)
        {
            return -Decimal18_6Max;
        }

        return Math.Truncate(value * 1_000_000m) / 1_000_000m;
    }
}