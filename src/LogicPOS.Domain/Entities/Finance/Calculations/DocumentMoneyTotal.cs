using System.Globalization;

namespace LogicPOS.Domain.Entities.Finance.Calculations;

/// <summary>
/// One money total per document for list, summary and payment — always <see cref="RoundingExtension.Round"/> (half away from zero).
/// </summary>
public static class DocumentMoneyTotal
{
    public static decimal Canonical(decimal storedTotalFinal) => storedTotalFinal.Round();

    public static decimal FromLines(IEnumerable<decimal> lineMoneyTotals)
        => lineMoneyTotals.Sum(amount => amount.Round()).Round();

    public static string Format(decimal storedTotalFinal, IFormatProvider? formatProvider = null)
        => Canonical(storedTotalFinal).ToString("N2", formatProvider ?? CultureInfo.CurrentCulture);

    public static string FormatFixed(decimal storedTotalFinal, CultureInfo culture)
        => Canonical(storedTotalFinal).ToString("0.00", culture);
}