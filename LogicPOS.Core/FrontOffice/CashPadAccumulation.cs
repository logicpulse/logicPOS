namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Pure cash-pad note accumulation for <c>PaymentWindow</c>.
/// Notes must add to the running total; reset belongs to open/clear only.
/// </summary>
public static class CashPadAccumulation
{
    /// <summary>
    /// Adds <paramref name="noteAmount"/> to the running total.
    /// When the user was typing digits, seed from the typed value (or zero if missing);
    /// otherwise keep <paramref name="currentTotal"/>.
    /// </summary>
    public static decimal ApplyNote(
        decimal currentTotal,
        decimal noteAmount,
        bool wasTyping,
        decimal? typedValue)
    {
        var baseTotal = wasTyping ? typedValue.GetValueOrDefault() : currentTotal;
        return baseTotal + noteAmount;
    }
}
