using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.FrontOffice;

public sealed class SplitShareLine
{
    public SplitShareLine(PosTicketLine line, decimal orderNetUnitPrice)
    {
        Line = line;
        OrderNetUnitPrice = orderNetUnitPrice;
    }

    public PosTicketLine Line { get; }

    /// <summary>Net price stored on the order, used to reduce that line after payment.</summary>
    public decimal OrderNetUnitPrice { get; }
}

/// <summary>
/// Builds the lines of one splitter so the payment total and the invoice total
/// both equal the amount shown on the split.
/// </summary>
public static class SplitBillShare
{
    public static IReadOnlyList<SplitShareLine> ForAmount(IReadOnlyList<PosTicketLine> lines, decimal amount, bool takeAll = false)
    {
        var bill = lines.Sum(line => line.Total);
        if (bill <= 0 || amount <= 0)
        {
            return [];
        }

        var requested = amount.Round();
        if (requested <= 0)
        {
            return [];
        }

        if (takeAll)
        {
            return Priced(lines.Where(line => line.Quantity > 0).Select(line => line.WithQuantity(line.Quantity)).ToList(), requested, adjustQuantity: false);
        }

        var target = Math.Min(requested, bill.Round());
        if (target >= bill.Round())
        {
            return lines
                .Where(line => line.Quantity > 0)
                .Select(line => new SplitShareLine(line.WithQuantity(line.Quantity), line.NetUnitPrice))
                .ToList();
        }

        var ratio = target / bill;
        var share = new List<PosTicketLine>();
        var caps = new List<decimal>();
        var orderPrices = new List<decimal>();
        foreach (var line in lines)
        {
            if (line.Quantity <= 0 || line.Total <= 0)
            {
                continue;
            }

            var quantity = decimal.Round(line.Quantity * ratio, 3, MidpointRounding.AwayFromZero);
            if (quantity <= 0)
            {
                continue;
            }

            if (quantity > line.Quantity)
            {
                quantity = line.Quantity;
            }

            var copy = line.WithQuantity(quantity);
            share.Add(copy);
            caps.Add(line.Quantity);
            orderPrices.Add(line.NetUnitPrice);
            AlignDisplay(copy);
        }

        return Priced(share, target, adjustQuantity: true, caps, orderPrices);
    }

    private static IReadOnlyList<SplitShareLine> Priced(
        List<PosTicketLine> share,
        decimal target,
        bool adjustQuantity,
        List<decimal>? caps = null,
        List<decimal>? orderPrices = null)
    {
        if (share.Count == 0)
        {
            return [];
        }

        caps ??= share.Select(line => line.Quantity).ToList();
        orderPrices ??= share.Select(line => line.NetUnitPrice).ToList();
        for (var i = 0; i < share.Count; i++)
        {
            AlignDisplay(share[i]);
        }

        CloseShownGap(share, caps, target, adjustQuantity);
        AbsorbDocumentResidue(share, target);
        return share.Select((line, index) => new SplitShareLine(line, orderPrices[index])).ToList();
    }

    private static void CloseShownGap(List<PosTicketLine> share, List<decimal> caps, decimal target, bool adjustQuantity)
    {
        for (var pass = 0; pass < 12; pass++)
        {
            var gap = (target - share.Sum(line => line.Total)).Round();
            if (gap == 0)
            {
                return;
            }

            var index = IndexOfLargest(share);
            var line = share[index];
            var desired = (line.Total + gap).Round();
            if (desired <= 0)
            {
                return;
            }

            if (adjustQuantity && TryQuantity(line, caps[index], desired))
            {
                continue;
            }

            SetRawTotal(line, desired);
            if (share.Sum(item => item.Total).Round() == target)
            {
                return;
            }
        }
    }

    private static bool TryQuantity(PosTicketLine line, decimal cap, decimal desired)
    {
        var original = line.Quantity;
        var unit = original > 0 ? line.Total / original : 0m;
        if (unit <= 0 || cap <= 0)
        {
            return false;
        }

        var seed = decimal.Round(desired / unit, 3, MidpointRounding.AwayFromZero);
        decimal? chosen = null;
        for (var step = -30; step <= 30; step++)
        {
            var quantity = decimal.Round(seed + (step * 0.001m), 3, MidpointRounding.AwayFromZero);
            if (quantity <= 0 || quantity > cap)
            {
                continue;
            }

            line.Quantity = quantity;
            if (line.Total != desired)
            {
                continue;
            }

            chosen = quantity;
            break;
        }

        if (chosen is decimal quantityFound)
        {
            line.Quantity = quantityFound;
            return true;
        }

        line.Quantity = original;
        return false;
    }

    private static void AbsorbDocumentResidue(List<PosTicketLine> share, decimal target)
    {
        if (share.Sum(line => line.Total).Round() != target)
        {
            return;
        }

        for (var pass = 0; pass < 4; pass++)
        {
            var residue = (DocumentRaw(share) - target).ToStoredPrecision();
            if (residue == 0)
            {
                return;
            }

            var pending = residue;
            foreach (var line in share)
            {
                if (pending == 0)
                {
                    break;
                }

                var raw = RawDisplay(line);
                var shown = line.Total;
                var low = shown - 0.004999m;
                var high = shown + 0.004999m;
                var room = pending > 0 ? raw - low : high - raw;
                if (room <= 0)
                {
                    continue;
                }

                var take = Math.Sign(pending) * Math.Min(Math.Abs(pending), room);
                var price = line.DisplayUnitPrice;
                var quantity = line.Quantity;
                ShiftRaw(line, -take);
                if (line.Total != shown)
                {
                    line.Quantity = quantity;
                    line.SetDisplayPrice(price);
                    continue;
                }

                pending = (pending - take).ToStoredPrecision();
            }

            if (pending == residue)
            {
                return;
            }
        }
    }

    private static void SetRawTotal(PosTicketLine line, decimal desired)
    {
        if (line.Quantity <= 0)
        {
            line.Quantity = 1;
        }

        var current = RawDisplay(line);
        if (current == 0)
        {
            line.SetDisplayPrice(desired / line.Quantity);
            current = RawDisplay(line);
        }

        for (var attempt = 0; attempt < 6 && current != 0 && current != desired; attempt++)
        {
            line.SetDisplayPrice(line.DisplayUnitPrice * (desired / current));
            current = RawDisplay(line);
        }
    }

    private static void ShiftRaw(PosTicketLine line, decimal delta)
    {
        if (line.Quantity == 0 || delta == 0)
        {
            return;
        }

        var before = RawDisplay(line);
        line.SetDisplayPrice(line.DisplayUnitPrice + (delta / line.Quantity));
        var actual = RawDisplay(line) - before;
        var error = delta - actual;
        if (actual != 0 && Math.Abs(error) >= 0.0000005m)
        {
            line.SetDisplayPrice(line.DisplayUnitPrice + (error / line.Quantity));
        }
    }

    private static void AlignDisplay(PosTicketLine line)
    {
        var target = RawNet(line);
        if (line.Quantity <= 0 || RawDisplay(line) == target)
        {
            return;
        }

        SetRawTotal(line, target);
    }

    private static int IndexOfLargest(List<PosTicketLine> share)
    {
        var index = 0;
        for (var i = 1; i < share.Count; i++)
        {
            if (share[i].Total > share[index].Total)
            {
                index = i;
            }
        }

        return index;
    }

    private static decimal DocumentRaw(IEnumerable<PosTicketLine> lines)
        => lines.Sum(RawDisplay).ToStoredPrecision();

    private static decimal RawNet(PosTicketLine line)
        => DocumentLineCalculator.Calculate(line.NetUnitPrice, line.Quantity, line.Discount, line.VatPercentage, useAgtRounding: false).TotalFinal;

    private static decimal RawDisplay(PosTicketLine line)
        => DocumentLineCalculator.CalculateLine(
            line.DisplayUnitPrice,
            line.PriceIncludesVat,
            line.Quantity,
            line.Discount,
            globalDiscount: 0,
            line.VatPercentage,
            useAgtRounding: false).TotalFinal;
}
