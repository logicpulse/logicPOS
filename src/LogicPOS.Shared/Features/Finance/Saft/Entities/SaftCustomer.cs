namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftCustomer
{
    public string Id { get; set; }  = null!;
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? ZipCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string FiscalNumber { get; set; } = null!;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
}