namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// PT faturação: FT/FS/FR só se anulam com motivo e sem recibo, nota de crédito nem estado faturado.
/// Rascunhos apagam-se (sem passar por anulado).
/// </summary>
public static class InvoiceCancellationGuard
{
    public static bool IsSalesInvoiceFamily(string? documentType)
    {
        var type = NormalizeAcronym(documentType);
        return type is "FT" or "FS" or "FR";
    }

    /// <summary>Returns a user-facing refusal, or <see langword="null"/> when cancellation may proceed.</summary>
    public static string? Validate(
        string? documentType,
        string? status,
        bool isDraft,
        bool hasReceipt,
        bool hasCreditNote,
        string? reason)
    {
        if (isDraft)
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            return "Indique o motivo da anulação.";
        }

        var statusCode = (status ?? string.Empty).Trim();
        if (statusCode is "A")
        {
            return "O documento já está anulado.";
        }

        if (IsSalesInvoiceFamily(documentType) == false)
        {
            return null;
        }

        if (statusCode is "F")
        {
            return "Não é possível anular um documento faturado.";
        }

        if (hasReceipt)
        {
            return "Não é possível anular uma fatura com recibo. Corrija com uma nota de crédito.";
        }

        if (hasCreditNote)
        {
            return "Não é possível anular uma fatura com nota de crédito.";
        }

        return null;
    }

    private static string NormalizeAcronym(string? acronym)
        => (acronym ?? string.Empty).Trim().ToUpperInvariant();
}