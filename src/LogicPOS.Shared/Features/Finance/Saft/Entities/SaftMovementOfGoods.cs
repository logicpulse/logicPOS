namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftMovementOfGoods
{
    public int NumberOfMovementLines => Movements.Count;
    public decimal TotalQuantityIssued => Movements.Sum(x => x.Lines.Sum(y => y.Quantity));
    public List<SaftMovementOfGood> Movements { get; set; } = null!;
}