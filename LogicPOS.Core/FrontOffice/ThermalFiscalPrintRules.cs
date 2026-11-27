using System.Diagnostics.CodeAnalysis;
using LogicPOS.Core.Fiscal;

namespace LogicPOS.Core.FrontOffice;

/// <summary>
/// Which thermal ticket fiscal sections are gated by PRINT_QRCODE vs always-on marks.
/// </summary>
public static class ThermalFiscalPrintRules
{
    /// <summary>
    /// ATCUD and the certification line always print when data and plugin supply them.
    /// PRINT_QRCODE only disables the QR image outside the Portugal certified fiscal edition.
    /// </summary>
    public static bool IsPortugalCertifiedFiscalEdition(IFiscalModule? module, bool isPortugal)
        => isPortugal && module is { IsAvailable: true };

    public static bool ShouldPrintQrImage(
        bool printQrCodePreference,
        [NotNullWhen(true)] string? atQrPayload,
        bool portugalCertifiedFiscalEdition)
        => string.IsNullOrWhiteSpace(atQrPayload) == false
            && (portugalCertifiedFiscalEdition || printQrCodePreference);

    /// <inheritdoc cref="ShouldPrintQrImage(bool, string?, bool)"/>
    public static bool ShouldPrintQrImage(bool printQrCodePreference, [NotNullWhen(true)] string? atQrPayload)
        => ShouldPrintQrImage(printQrCodePreference, atQrPayload, portugalCertifiedFiscalEdition: false);
}