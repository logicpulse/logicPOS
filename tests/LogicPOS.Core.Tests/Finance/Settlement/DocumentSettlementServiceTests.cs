using LogicPOS.Domain.Entities.Finance.Documents.Receipts.Settlement;

namespace LogicPOS.Core.Tests.Finance.Settlement;

public class DocumentSettlementServiceTests
{
    [Fact]
    public void ComputeSettleableAmount_delegates_to_document_line_calculator()
    {
        var amount = DocumentSettlementService.ComputeSettleableAmount([10.005m, 10.005m], []);
        Assert.Equal(20.02m, amount);
    }

    [Fact]
    public void AllocateInvoicePayments_sets_tax_and_withholding_from_invoice_totals_only()
    {
        var invoiceId = Guid.NewGuid();
        var docs = new List<DocumentSettlementService.SettlementDocument>
        {
            new(
                invoiceId,
                "FT",
                TotalFinal: 100m,
                TotalTax: 23m,
                WithholdingTaxAmount: 6.5m,
                TotalPaid: 0m,
                CreatedAt: DateTime.UtcNow),
        };

        var allocations = DocumentSettlementService.AllocateInvoicePayments(docs, receiptCashAmount: 50m, creditNotePoolTotal: 0m);

        Assert.Single(allocations);
        Assert.Equal(invoiceId, allocations[0].DocumentId);
        Assert.Equal(50m, allocations[0].AmountToPay);
        Assert.Equal(11.50m, allocations[0].TaxPayable);
        Assert.Equal(3.25m, allocations[0].WithholdingTaxAmount);
        Assert.Equal(11.50m, DocumentSettlementService.SumTaxPayable(allocations));
        Assert.Equal(3.25m, DocumentSettlementService.SumWithholding(allocations));
    }
}
