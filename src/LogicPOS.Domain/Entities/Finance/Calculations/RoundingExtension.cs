using System.Globalization;

namespace LogicPOS.Domain.Entities.Finance.Calculations;

public static class RoundingExtension
{
    /// <summary>Max absolute value for SQL <c>decimal(18,6)</c> columns.</summary>
    public const decimal Decimal18_6Max = 999_999_999_999.999999m;

    /// <summary>
    /// Divisions such as price / 1.23 leave noise in the last of 28 digits (7.50 becomes 7.4999…).
    /// Money never carries meaning past 12 decimals, so cutting operations drop that noise first.
    /// </summary>
    private const int NoiseDecimals = 12;

    /// <summary>Fractional digits kept by the <c>decimal(18,6)</c> columns.</summary>
    public const int StoredDecimals = 6;

    /// <summary>
    /// Totals must already fit the column before they are signed: each database cuts extra digits
    /// differently (SQLite keeps a double, MySQL rounds, SQL Server may truncate), so the exported
    /// total would otherwise differ from the signed one.
    /// </summary>
    public static decimal ToStoredPrecision(this decimal value) =>
        Math.Round(value, StoredDecimals, MidpointRounding.AwayFromZero);

    /// <summary>Two decimals, midpoint away from zero, used for signed and exported totals.</summary>
    public static decimal Round(this decimal value) =>
        Math.Round(Math.Round(value, NoiseDecimals), 2, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Money written to SAF-T, the document hash and the AT/AGT services: rounded by <see cref="Round"/>
    /// before it is printed, so the text never depends on the scale the value happens to carry.
    /// </summary>
    public static string To2DecimalPlacesString(this decimal value) =>
        value.Round().ToString("0.00", CultureInfo.InvariantCulture);

    public static decimal ApplyAgtRounding(this decimal value) =>
        Math.Ceiling(Math.Round(value, NoiseDecimals) * 100) / 100;

    public static decimal TruncateDecimal(this decimal value) =>
        Math.Truncate(Math.Round(value, NoiseDecimals) * 100) / 100;

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
