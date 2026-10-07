namespace LogicPOS.Core.FrontOffice;

public sealed class PosCustomer
{
    public PosCustomer(
        Guid id,
        string name,
        string fiscalNumber,
        string? cardNumber,
        decimal discount,
        string? address,
        string? locality,
        string? zipCode,
        string? city,
        string? country,
        bool isFinalConsumer,
        string? phone = null,
        string? email = null,
        string? notes = null,
        Guid customerTypeId = default,
        Guid priceTypeId = default,
        Guid countryId = default,
        string? code = null)
    {
        Id = id;
        Name = name;
        FiscalNumber = fiscalNumber;
        CardNumber = cardNumber;
        Discount = discount;
        Address = address;
        Locality = locality;
        ZipCode = zipCode;
        City = city;
        Country = country;
        IsFinalConsumer = isFinalConsumer;
        Phone = phone;
        Email = email;
        Notes = notes;
        CustomerTypeId = customerTypeId;
        PriceTypeId = priceTypeId;
        CountryId = countryId;
        Code = code;
    }

    public Guid Id { get; }

    public string? Code { get; }

    public string Name { get; }

    public string FiscalNumber { get; }

    public string? CardNumber { get; }

    public decimal Discount { get; }

    public string? Address { get; }

    public string? Locality { get; }

    public string? ZipCode { get; }

    public string? City { get; }

    public string? Country { get; }

    public bool IsFinalConsumer { get; }

    public string? Phone { get; }

    public string? Email { get; }

    public string? Notes { get; }

    public Guid CustomerTypeId { get; }

    public Guid PriceTypeId { get; }

    public Guid CountryId { get; }
}
