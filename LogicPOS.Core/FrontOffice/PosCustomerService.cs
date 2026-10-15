using System.Globalization;
using System.Text;
using LogicPOS.Domain.Entities;
using LogicPOS.Persistence.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogicPOS.Core.FrontOffice;

public sealed class PosCustomerService : IPosCustomerService
{
    private const string FinalConsumerName = "Consumidor Final";

    private readonly IServiceScopeFactory _scopes;
    private readonly object _cacheGate = new();
    private int _cacheVersion;
    private CachedCustomers? _cache;

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

        var folded = Fold(term);
        var customers = await CachedAsync(cancellationToken);
        return customers
            .Where(item => item.Name.Contains(folded, StringComparison.Ordinal)
                || item.FiscalNumber.Contains(folded, StringComparison.Ordinal)
                || item.CardNumber.Contains(folded, StringComparison.Ordinal))
            .OrderBy(item => item.Customer.Name)
            .Take(20)
            .Select(item => item.Customer)
            .ToList();
    }

    /// <summary>Drops the decrypted search list after a customer is saved or deleted.</summary>
    public void InvalidateSearch()
    {
        lock (_cacheGate)
        {
            _cache = null;
            _cacheVersion++;
        }
    }

    private async Task<IReadOnlyList<CachedCustomer>> CachedAsync(CancellationToken cancellationToken)
    {
        int version;
        lock (_cacheGate)
        {
            if (_cache is not null)
            {
                return _cache.Rows;
            }

            version = _cacheVersion;
        }

        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        // Name and fiscal number are encrypted, so accent folding happens after decryption.
        var customers = await database.Customers.AsNoTracking()
            .Where(item => item.IsDeleted == false)
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

        var rows = customers
            .Select(item => new CachedCustomer(
                Map(item.Id, item.Name, item.FiscalNumber, item.CardNumber, item.Discount, item.Address, item.Locality, item.ZipCode, item.City, item.Country),
                Fold(item.Name),
                Fold(item.FiscalNumber),
                Fold(item.CardNumber)))
            .ToList();

        lock (_cacheGate)
        {
            if (_cacheVersion == version)
            {
                _cache = new CachedCustomers(rows);
            }

            return _cache?.Rows ?? rows;
        }
    }

    private static string Fold(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var decomposed = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(decomposed.Length);
        foreach (var character in decomposed)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        return builder.ToString().ToLowerInvariant();
    }

    private sealed class CachedCustomers(List<CachedCustomer> rows)
    {
        public List<CachedCustomer> Rows { get; } = rows;
    }

    private sealed record CachedCustomer(PosCustomer Customer, string Name, string FiscalNumber, string CardNumber);

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
