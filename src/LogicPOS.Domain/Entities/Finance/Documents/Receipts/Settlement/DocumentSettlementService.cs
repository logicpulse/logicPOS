using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Domain.Entities.Finance.Documents.Receipts.Settlement;

/// <summary>
/// Receipt settlement math without persistence, QR, or hash (used by PayDocuments / local receipt emission).
/// </summary>
public static class DocumentSettlementService
{
    public static decimal ComputeSettleableAmount(
        IEnumerable<decimal> invoicesToPay,
        IEnumerable<decimal> creditNotesTotal)
        => DocumentLineCalculator.SettlementAmount(invoicesToPay, creditNotesTotal);

    public sealed record SettlementDocument(
        Guid Id,
        string Type,
        decimal TotalFinal,
        decimal TotalTax,
        decimal WithholdingTaxAmount,
        decimal TotalPaid,
        DateTime CreatedAt);

    public sealed record InvoicePaymentAllocation(
        Guid DocumentId,
        decimal AmountToPay,
        decimal TaxPayable,
        decimal WithholdingTaxAmount);

    /// <summary>
    /// Allocates receipt cash and pooled credit-note totals across FT/ND invoices (legacy PayDocuments order).
    /// </summary>
    public static IReadOnlyList<InvoicePaymentAllocation> AllocateInvoicePayments(
        IReadOnlyList<SettlementDocument> documents,
        decimal receiptCashAmount,
        decimal creditNotePoolTotal)
    {
        var invoices = documents
            .Where(d => d.Type is "FT" or "ND")
            .OrderBy(d => d.CreatedAt)
            .ToList();

        var totalCredit = creditNotePoolTotal.Round();
        var allocations = new List<InvoicePaymentAllocation>();
        var remainingCash = receiptCashAmount;

        foreach (var invoice in invoices)
        {
            var outstanding = (invoice.TotalFinal - invoice.TotalPaid).Round();
            if (outstanding <= 0)
            {
                continue;
            }

            var fromCredit = Math.Min(totalCredit, outstanding);
            totalCredit -= fromCredit;
            var remainingOutstanding = outstanding - fromCredit;
            var fromCash = Math.Min(remainingCash, remainingOutstanding);
            remainingCash -= fromCash;
            var amountToPay = (fromCredit + fromCash).Round();
            if (amountToPay <= 0)
            {
                continue;
            }

            allocations.Add(new InvoicePaymentAllocation(
                invoice.Id,
                amountToPay,
                TaxPayableForAmount(invoice.TotalFinal, invoice.TotalTax, amountToPay),
                WithholdingForAmount(invoice.TotalFinal, invoice.WithholdingTaxAmount, amountToPay)));
        }

        return allocations;
    }

    public static decimal SumTaxPayable(IEnumerable<InvoicePaymentAllocation> allocations)
        => allocations.Sum(a => a.TaxPayable).Round();

    public static decimal SumWithholding(IEnumerable<InvoicePaymentAllocation> allocations)
        => allocations.Sum(a => a.WithholdingTaxAmount).Round();

    internal static decimal TaxPayableForAmount(decimal totalFinal, decimal totalTax, decimal amount)
    {
        if (totalFinal == 0)
        {
            return 0;
        }

        return (amount * totalTax / totalFinal).Round();
    }

    internal static decimal WithholdingForAmount(decimal totalFinal, decimal withholdingTaxAmount, decimal amount)
    {
        if (totalFinal == 0)
        {
            return 0;
        }

        return (amount * withholdingTaxAmount / totalFinal).Round();
    }
}
