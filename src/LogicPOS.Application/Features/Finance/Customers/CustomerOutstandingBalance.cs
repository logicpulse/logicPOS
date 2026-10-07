namespace LogicPOS.Application.Features.Finance.Customers;

/// <summary>
/// Customer current-account balance from open FT/ND amounts (TotalFinal − paid allocations).
/// <see cref="Balance"/> keeps the legacy sign: credit − debit (negative = customer owes).
/// </summary>
public sealed record CustomerOutstandingBalance(
    decimal TotalDebit,
    decimal TotalCredit,
    decimal Balance)
{
    public static CustomerOutstandingBalance Empty { get; } = new(0, 0, 0);
}
