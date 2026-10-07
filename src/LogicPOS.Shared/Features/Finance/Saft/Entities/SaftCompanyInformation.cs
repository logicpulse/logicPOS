namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public struct SaftCompanyInformation
{
    public string FiscalNumber { get; set; }
    public string? Name { get; set; }
    public string? BusinessName { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? CountryCode2 { get; set; }
    public string? TaxEntity { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string CurrencyCode { get; set; }
    public string? PostalCode { get; set; }
    public string? Region { get; set; }
}
