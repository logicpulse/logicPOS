using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.FrontOffice;

public sealed class PosCustomerService : IPosCustomerService
{
    private const string FinalConsumerName = "Consumidor Final";

    private readonly IServiceScopeFactory _scopes;

    public PosCustomerService(IServiceScopeFactory scopes)
    {
        _scopes = scopes;
    }

    public async Task<PosCustomer?> GetFinalConsumerAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();

        var customer = await database.Customers
            .AsNoTracking()
            .Where(item => item.IsDeleted == false && item.Name == FinalConsumerName)
            .Select(item => new
            {
                item.Id,
                item.Name,
                item.FiscalNumber,
                item.CardNumber,
                item.Discount,
                item.Address,
                item.Locality,
                item.ZipCode,
                item.City,
                Country = item.Country != null ? item.Country.Designation : null
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (customer is null)
        {
            return null;
        }

        return new PosCustomer(
            customer.Id,
            customer.Name,
            customer.FiscalNumber,
            customer.CardNumber,
            customer.Discount,
            customer.Address,
            customer.Locality,
            customer.ZipCode,
            customer.City,
            customer.Country,
            true);
    }

    public async Task<PosCustomer?> FindAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var customer = await database.Customers.AsNoTracking()
            .Where(item => item.Id == id && item.IsDeleted == false)
            .Select(item => new
            {
                item.Id,
                item.Name,
                item.FiscalNumber,
                item.CardNumber,
                item.Discount,
                item.Address,
                item.Locality,
                item.ZipCode,
                item.City,
                Country = item.Country != null ? item.Country.Designation : null
            })
            .FirstOrDefaultAsync(cancellationToken);
        return customer is null ? null : Map(customer.Id, customer.Name, customer.FiscalNumber, customer.CardNumber, customer.Discount, customer.Address, customer.Locality, customer.ZipCode, customer.City, customer.Country);
    }

    public async Task<PosCustomer?> FindByCardAsync(string cardNumber, CancellationToken cancellationToken = default)
    {
        var matches = await SearchAsync(cardNumber, cancellationToken);
        return matches.FirstOrDefault(customer => string.Equals(customer.CardNumber, cardNumber, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<PosCustomer>> SearchAsync(string text, CancellationToken cancellationToken = default)
    {
        var term = text.Trim();
        if (term.Length == 0)
        {
            return [];
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        var customers = await database.Customers.AsNoTracking()
            .Where(item => item.IsDeleted == false)
            .Where(item => item.Name.Contains(term) || item.FiscalNumber.Contains(term) || (item.CardNumber != null && item.CardNumber.Contains(term)))
            .OrderBy(item => item.Name)
            .Take(20)
            .Select(item => new
            {
                item.Id,
                item.Name,
                item.FiscalNumber,
                item.CardNumber,
                item.Discount,
                item.Address,
                item.Locality,
                item.ZipCode,
                item.City,
                Country = item.Country != null ? item.Country.Designation : null
            })
            .ToListAsync(cancellationToken);

        return customers
            .Select(item => Map(item.Id, item.Name, item.FiscalNumber, item.CardNumber, item.Discount, item.Address, item.Locality, item.ZipCode, item.City, item.Country))
            .ToList();
    }

    private static PosCustomer Map(
        Guid id,
        string name,
        string fiscalNumber,
        string? cardNumber,
        decimal discount,
        string? address,
        string? locality,
        string? zipCode,
        string? city,
        string? country)
        => new(id, name, fiscalNumber, cardNumber, discount, address, locality, zipCode, city, country, name == FinalConsumerName);
}
