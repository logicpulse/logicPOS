using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.FrontOffice;

public readonly record struct OrderLineRemaining(decimal Quantity, decimal NetUnitPrice);

/// <summary>
/// Quantity and price left on the order after a split payment, so the remaining
/// total is exactly the money still owed.
/// </summary>
public static class OrderSplitBalance
{
    public static IReadOnlyList<OrderLineRemaining> Remaining(IReadOnlyList<PosTicketLine> lines, decimal remainingTotal)
    {
        if (lines.Count == 0)
        {
            return [];
        }

        var bill = lines.Sum(line => line.Total).Round();
        var target = remainingTotal.Round();
        if (target <= 0)
        {
            return lines.Select(line => new OrderLineRemaining(0m, line.NetUnitPrice)).ToList();
        }

        if (target >= bill)
        {
            return lines.Select(line => new OrderLineRemaining(line.Quantity, line.NetUnitPrice)).ToList();
        }

        var ratio = target / bill;
        var copies = new List<PosTicketLine>();
        foreach (var line in lines)
        {
            var quantity = decimal.Round(line.Quantity * ratio, 6, MidpointRounding.AwayFromZero);
            if (quantity <= 0 && line.Quantity > 0 && line.Total > 0)
            {
                quantity = 0.000001m;
            }

            if (quantity > line.Quantity)
            {
                quantity = line.Quantity;
            }

            copies.Add(line.WithQuantity(quantity));
        }

        FitTotal(copies, target);
        return copies.Select(line => new OrderLineRemaining(line.Quantity, line.NetUnitPrice)).ToList();
    }

    private static void FitTotal(List<PosTicketLine> lines, decimal target)
    {
        for (var pass = 0; pass < 8; pass++)
        {
            var gap = (target - lines.Sum(line => line.Total)).Round();
            if (gap == 0)
            {
                return;
            }

            var index = 0;
            for (var i = 1; i < lines.Count; i++)
            {
                if (lines[i].Total > lines[index].Total)
                {
                    index = i;
                }
            }

            var line = lines[index];
            if (line.Quantity <= 0 || line.Total <= 0)
            {
                return;
            }

            var desired = (line.Total + gap).Round();
            if (desired <= 0)
            {
                return;
            }

            var raw = DocumentLineCalculator.Calculate(line.NetUnitPrice, line.Quantity, line.Discount, line.VatPercentage, useAgtRounding: false).TotalFinal;
            if (raw == 0)
            {
                return;
            }

            line.NetUnitPrice = (line.NetUnitPrice * (desired / raw)).ToStoredPrecision();
        }
    }
}
