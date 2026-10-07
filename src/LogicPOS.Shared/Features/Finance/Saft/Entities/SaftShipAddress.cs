namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftShipAddress
{
    public string? DeliveryID { get; set; }
    public DateTime? DeliveryDate { get; set; } 
    public string? WarehouseID { get; set; }
    public string? LocationID { get; set; }
    public string? AddressDetail { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Region {get;set;}
    public string? Country { get; set; }
}