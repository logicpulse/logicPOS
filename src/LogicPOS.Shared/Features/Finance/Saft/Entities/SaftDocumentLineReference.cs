namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftDocumentLineReference {
    public string Reference { get; set; } = null!;
    public string Reason { get; set; } = null!;
}