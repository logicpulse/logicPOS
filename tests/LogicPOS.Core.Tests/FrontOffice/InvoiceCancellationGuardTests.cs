using LogicPOS.Core.FrontOffice;

namespace LogicPOS.Core.Tests.FrontOffice;

public sealed class InvoiceCancellationGuardTests
{
    [Fact]
    public void Draft_skips_reason_and_invoice_blocks()
    {
        Assert.Null(InvoiceCancellationGuard.Validate("FT", "N", isDraft: true, hasReceipt: true, hasCreditNote: true, reason: null));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Issued_document_without_reason_is_refused(string? reason)
    {
        var error = InvoiceCancellationGuard.Validate("FT", "N", isDraft: false, hasReceipt: false, hasCreditNote: false, reason);
        Assert.Equal("Indique o motivo da anulação.", error);
    }

    [Fact]
    public void FT_with_receipt_is_refused()
    {
        var error = InvoiceCancellationGuard.Validate("FT", "N", isDraft: false, hasReceipt: true, hasCreditNote: false, reason: "Erro");
        Assert.Contains("recibo", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FS_with_credit_note_is_refused()
    {
        var error = InvoiceCancellationGuard.Validate("FS", "N", isDraft: false, hasReceipt: false, hasCreditNote: true, reason: "Devolução");
        Assert.Contains("nota de crédito", error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FR_with_invoiced_status_is_refused()
    {
        var error = InvoiceCancellationGuard.Validate("FR", "F", isDraft: false, hasReceipt: false, hasCreditNote: false, reason: "Motivo");
        Assert.Contains("faturado", error, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("FT")]
    [InlineData("FS")]
    [InlineData("FR")]
    public void Clean_sales_invoice_may_be_cancelled(string type)
    {
        Assert.Null(InvoiceCancellationGuard.Validate(type, "N", isDraft: false, hasReceipt: false, hasCreditNote: false, reason: "Motivo"));
    }

    [Fact]
    public void Guide_may_be_cancelled_with_reason_only()
    {
        Assert.Null(InvoiceCancellationGuard.Validate("GR", "N", isDraft: false, hasReceipt: true, hasCreditNote: true, reason: "Motivo"));
    }

    [Fact]
    public void Already_cancelled_is_refused()
    {
        var error = InvoiceCancellationGuard.Validate("GR", "A", isDraft: false, hasReceipt: false, hasCreditNote: false, reason: "Motivo");
        Assert.Equal("O documento já está anulado.", error);
    }
}