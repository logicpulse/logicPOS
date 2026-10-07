namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftMovementOfGood : SaftDocument
{
    public Guid? OriginDocumentId { get; set; }
    public SaftShipAddress ShipTo { get; set; } = null!;
    public SaftShipAddress ShipFrom { get; set; } = null!;
    public DateTime MovementEndTime { get; set; }
    public DateTime MovementStartTime { get; set; }
}