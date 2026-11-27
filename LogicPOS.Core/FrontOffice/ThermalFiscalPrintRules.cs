namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Which thermal ticket fiscal sections are gated by PRINT_QRCODE vs always-on marks.
/// </summary>
public static class ThermalFiscalPrintRules
{
    /// <summary>
    /// ATCUD and the certification line always print when data and plugin supply them.
    /// Only the QR image obeys the PRINT_QRCODE preference.
    /// </summary>
    public static bool ShouldPrintQrImage(bool printQrCodePreference, string? atQrPayload)
        => printQrCodePreference && string.IsNullOrWhiteSpace(atQrPayload) == false;
}