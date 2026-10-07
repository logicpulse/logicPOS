using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Repositories;
using LogicPOS.Domain.ValueObjects;
using LogicPOS.Persistence.Database;
using LogicPOS.Persistence.Repositories.Common;

using Microsoft.EntityFrameworkCore;

namespace LogicPOS.Persistence.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(LogicPOSDbContext context) : base(context)
    {
    }

    public async Task<Guid> CreateCustomerFromDocumentAsync(DocumentCustomer documentCustomer, CancellationToken ct = default)
    {
        var customer = new Customer();
        customer.Order = await GetNextOrderAsync(ct);
        customer.Code = await GetNextCodeAsync(ct);
        customer.CustomerTypeId = await Database.CustomerTypes!.Select( x => x.Id).FirstAsync();
        customer.PriceTypeId = await Database.PriceTypes!.Select( x => x.Id).FirstAsync();
        customer.Name = documentCustomer.Name;
        customer.Address = documentCustomer.Address;
        customer.FiscalNumber = documentCustomer.FiscalNumber;
        customer.ZipCode = documentCustomer.ZipCode;
        customer.City = documentCustomer.City;
        customer.CountryId = documentCustomer.CountryId;
        customer.Locality = documentCustomer.Locality;
        customer.Email = documentCustomer.Email;
        customer.Phone = documentCustomer.Phone;
        customer.Fax = documentCustomer.Fax;

        await Database.Customers!.AddAsync(customer, ct);

        return customer.Id;
    }

    public async Task<DocumentCustomer> CreateDocumentCustomerAsync(Guid customerId, CancellationToken ct = default)
    {
        var customer = await Database.Customers!
            .Include(x => x.Country)
            .AsNoTracking()
            .FirstAsync(x => x.Id == customerId, ct);

        return new DocumentCustomer
        {
            Name = customer.Name,
            Address = customer.Address,
            Locality = customer.Locality,
            ZipCode = customer.ZipCode,
            City = customer.City,
            Country = customer.Country!.Code2,
            CountryId = customer.CountryId,
            FiscalNumber = customer.FiscalNumber,
            Email = customer.Email,
            Phone = customer.Phone,
            Fax = customer.Fax
        };
    }

    public async Task<bool> FiscalNumberExistsAsync(string fiscalNumber, CancellationToken ct = default)
    {
        return await Database.Customers!
            .AnyAsync(x => x.FiscalNumber == fiscalNumber, ct);
    }

    public async Task<(string Name, string FiscalNumber)> GetCustomerNameAndFiscalNumberAsync(Guid customerId, CancellationToken ct = default)
    {
        var customer = await Database.Customers!
            .Where(x => x.Id == customerId)
            .Select(x => new { x.Name, x.FiscalNumber })
            .FirstAsync(ct);
        
        return (customer.Name, customer.FiscalNumber);
    }

    public async Task<bool> CardNumberExistsAsync(string cardNumber, CancellationToken ct = default)
    {
        return await Database.Customers.AnyAsync(x => x.CardNumber == cardNumber, ct);
    }

  

    public override async Task<bool> IsReferencedAsync(Guid id, CancellationToken ct = default)
    {
        return await Database.Documents!.AnyAsync(x => x.CustomerId == id, ct);
    }

    public async Task<bool> NameExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Database.Customers!
            .AnyAsync(x => x.Name == name, cancellationToken);
    }

    public async Task<Guid?> GetFinalConsumerIdAsync(CancellationToken ct = default)
    {
        return await Database.Customers!
            .Where(c => c.Id == CustomerWellKnownIds.FinalConsumer && !c.IsDeleted)
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> IsFinalConsumerAsync(Guid customerId, CancellationToken ct = default)
    {
        var finalConsumerId = await GetFinalConsumerIdAsync(ct);
        return finalConsumerId.HasValue && customerId == finalConsumerId.Value;
    }
}