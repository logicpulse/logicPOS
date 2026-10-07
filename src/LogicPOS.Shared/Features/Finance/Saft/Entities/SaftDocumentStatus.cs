namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftDocumentStatus
{
    public string Status { get; set; } = null!;
    public DateTime StatusDate { get; set; }
    public string? Reason { get; set; } 
    public string SourceID { get; set; } = null!;
    public string SourceBilling { get; set; } = null!;
}