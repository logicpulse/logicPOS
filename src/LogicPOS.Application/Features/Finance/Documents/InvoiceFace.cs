using LogicPOS.Domain.ValueObjects;

namespace LogicPOS.Application.Features.Finance.Documents;

/// <summary>
/// NIF impresso na face de FS e FR. O 999999990 do consumidor final fica no SAF-T e no QR.
/// </summary>
public static class InvoiceFace
{
    public static string? FiscalNumber(string? documentType, string? storedFiscalNumber)
    {
        var type = documentType?.Trim().ToUpperInvariant();
        if (type is not ("FS" or "FR"))
        {
            return storedFiscalNumber;
        }

        if (string.IsNullOrWhiteSpace(storedFiscalNumber))
        {
            return null;
        }

        var trimmed = storedFiscalNumber.Trim();
        return string.Equals(trimmed, PortugueseNif.FinalConsumer, StringComparison.OrdinalIgnoreCase)
            ? null
            : trimmed;
    }
}
