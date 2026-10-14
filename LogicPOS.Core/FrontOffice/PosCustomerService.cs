using LogicPOS.Domain.Entities;
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

        var folded = Fold(term);
        await using var scope = _scopes.CreateAsyncScope();
        var database = scope.ServiceProvider.GetRequiredService<LogicPOSDbContext>();
        // Name and fiscal number are encrypted, so a SQL replace cannot fold accents.
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

        return customers
            .Where(item => Hits(item.Name, folded) || Hits(item.FiscalNumber, folded) || Hits(item.CardNumber, folded))
            .OrderBy(item => item.Name)
            .Take(20)
            .Select(item => Map(item.Id, item.Name, item.FiscalNumber, item.CardNumber, item.Discount, item.Address, item.Locality, item.ZipCode, item.City, item.Country))
            .ToList();
    }

    private static bool Hits(string? value, string folded)
        => string.IsNullOrEmpty(value) == false && Fold(value).Contains(folded, StringComparison.Ordinal);

    private static string Fold(string value)
    {
        var text = value;
        foreach (var (from, to) in Accents)
        {
            text = text.Replace(from, to);
        }

        return text.ToLowerInvariant();
    }

    private static readonly (string From, string To)[] Accents =
    [
        ("á", "a"), ("à", "a"), ("â", "a"), ("ã", "a"), ("ä", "a"),
        ("Á", "A"), ("À", "A"), ("Â", "A"), ("Ã", "A"), ("Ä", "A"),
        ("é", "e"), ("è", "e"), ("ê", "e"), ("ë", "e"),
        ("É", "E"), ("È", "E"), ("Ê", "E"), ("Ë", "E"),
        ("í", "i"), ("ì", "i"), ("î", "i"), ("ï", "i"),
        ("Í", "I"), ("Ì", "I"), ("Î", "I"), ("Ï", "I"),
        ("ó", "o"), ("ò", "o"), ("ô", "o"), ("õ", "o"), ("ö", "o"),
        ("Ó", "O"), ("Ò", "O"), ("Ô", "O"), ("Õ", "O"), ("Ö", "O"),
        ("ú", "u"), ("ù", "u"), ("û", "u"), ("ü", "u"),
        ("Ú", "U"), ("Ù", "U"), ("Û", "U"), ("Ü", "U"),
        ("ç", "c"), ("Ç", "C"), ("ñ", "n"), ("Ñ", "N")
    ];

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
