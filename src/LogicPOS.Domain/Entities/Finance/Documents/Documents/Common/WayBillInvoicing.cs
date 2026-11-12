namespace LogicPOS.Domain.Entities.Documents;

/// <summary>A waybill is invoiced whole by a single FT, FS or FR, which marks it as invoiced.</summary>
public static class WayBillInvoicing
{
    public const string InvoicedStatus = "F";

    /// <summary>Whether the invoice bills every article of the waybill in the same quantity, and nothing else.</summary>
    public static bool BillsWholeWayBill(
        IEnumerable<(Guid ArticleId, decimal Quantity)> wayBillLines,
        IEnumerable<(Guid ArticleId, decimal Quantity)> invoiceLines)
    {
        var wayBillQuantities = SumByArticle(wayBillLines);
        var invoiceQuantities = SumByArticle(invoiceLines);

        return wayBillQuantities.Count == invoiceQuantities.Count
               && wayBillQuantities.All(line =>
                   invoiceQuantities.TryGetValue(line.Key, out var quantity) && quantity == line.Value);
    }

    private static Dictionary<Guid, decimal> SumByArticle(IEnumerable<(Guid ArticleId, decimal Quantity)> lines) =>
        lines.GroupBy(line => line.ArticleId)
            .ToDictionary(group => group.Key, group => group.Sum(line => line.Quantity));
}
