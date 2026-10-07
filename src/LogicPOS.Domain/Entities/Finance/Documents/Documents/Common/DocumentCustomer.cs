using System.ComponentModel.DataAnnotations.Schema;

namespace LogicPOS.Domain.ValueObjects;

[ComplexType]
public sealed class DocumentCustomer
{
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Locality { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public Guid CountryId { get; set; }
    public string FiscalNumber { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
}