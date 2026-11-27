namespace LogicPOS.Core.Fiscal;

/// <summary>
/// AT QR on print: persisted payload wins; the fiscal plugin fills only when storage is empty.
/// </summary>
public static class FiscalQrOnPrint
{
    public static void ResolveForPrint(
        string? storedAtQrCode,
        IFiscalModule? module,
        FiscalDocument document,
        out string? fiscalCodeLine,
        out string? atQrCodeForOutput)
    {
        var mark = FiscalMarks.Read(module, document);
        fiscalCodeLine = string.IsNullOrWhiteSpace(mark.CodeLine) ? null : mark.CodeLine;

        if (string.IsNullOrWhiteSpace(storedAtQrCode) == false)
        {
            atQrCodeForOutput = storedAtQrCode;
            return;
        }

        atQrCodeForOutput = string.IsNullOrWhiteSpace(mark.QrPayload) ? null : mark.QrPayload;
    }
}