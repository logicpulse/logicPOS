using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Application.Features.Finance.Customers;

public static class CustomerOutstandingBalanceCompute
{
    public readonly record struct DocumentBalanceRow(Guid Id, decimal TotalFinal);

    public static (decimal TotalDebit, decimal Outstanding, decimal TotalCredit, decimal Balance) Compute(
        IEnumerable<DocumentBalanceRow> documents,
        IReadOnlyDictionary<Guid, decimal> paidByDocumentId)
    {
        var rows = documents as IList<DocumentBalanceRow> ?? documents.ToList();
        var totalDebit = rows.Sum(d => d.TotalFinal.Round());
        var outstanding = rows.Sum(d => (d.TotalFinal - paidByDocumentId.GetValueOrDefault(d.Id)).Round());
        var totalCredit = (totalDebit - outstanding).Round();
        var balance = (totalCredit - totalDebit).Round();
        return (totalDebit, outstanding, totalCredit, balance);
    }

    public static CustomerOutstandingBalance ToCustomerOutstandingBalance(
        IEnumerable<DocumentBalanceRow> documents,
        IReadOnlyDictionary<Guid, decimal> paidByDocumentId)
    {
        var (totalDebit, _, totalCredit, balance) = Compute(documents, paidByDocumentId);
        return new CustomerOutstandingBalance(totalDebit, totalCredit, balance);
    }
}
