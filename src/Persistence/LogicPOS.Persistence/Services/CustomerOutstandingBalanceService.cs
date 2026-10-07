using LogicPOS.Application.Features.Finance.Customers;
using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Domain.Repositories;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Services;

public sealed class CustomerOutstandingBalanceService : ICustomerOutstandingBalanceService
{
    private readonly LogicPOSDbContext _database;
    private readonly IDocumentRepository _documentRepository;

    public CustomerOutstandingBalanceService(
        LogicPOSDbContext database,
        IDocumentRepository documentRepository)
    {
        _database = database;
        _documentRepository = documentRepository;
    }

    public async Task<CustomerOutstandingBalance> GetLifetimeBalanceAsync(
        Guid customerId,
        CancellationToken ct = default)
    {
        var breakdown = await ComputeBalancesAsync([customerId], ct);
        return breakdown.GetValueOrDefault(customerId, CustomerOutstandingBalance.Empty);
    }

    public async Task<IReadOnlyDictionary<Guid, decimal>> GetLifetimeBalancesAsync(
        IEnumerable<Guid> customerIds,
        CancellationToken ct = default)
    {
        var breakdown = await ComputeBalancesAsync(customerIds, ct);
        return breakdown.ToDictionary(x => x.Key, x => x.Value.Balance);
    }

    private async Task<Dictionary<Guid, CustomerOutstandingBalance>> ComputeBalancesAsync(
        IEnumerable<Guid> customerIds,
        CancellationToken ct)
    {
        var ids = customerIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return [];
        }

        var documentRows = await _database.Documents!
            .Where(d => ids.Contains(d.CustomerId))
            .ApplyActiveSalesInvoicesFilter()
            .Where(d => d.Type == "FT" || d.Type == "ND")
            .Select(d => new { d.Id, d.CustomerId, d.TotalFinal })
            .ToListAsync(ct);

        var paidTotals = await _documentRepository.GetDocumentsTotalPaidAsync(
            documentRows.Select(d => d.Id),
            ct);

        return ids.ToDictionary(
            id => id,
            id =>
            {
                var customerDocs = documentRows.Where(d => d.CustomerId == id).ToList();
                var totalDebit = customerDocs.Sum(d => d.TotalFinal).Round();
                var outstanding = customerDocs
                    .Sum(d => (d.TotalFinal - paidTotals.GetValueOrDefault(d.Id)).Round());
                var totalCredit = (totalDebit - outstanding).Round();
                var balance = (totalCredit - totalDebit).Round();
                return new CustomerOutstandingBalance(totalDebit, totalCredit, balance);
            });
    }
}
