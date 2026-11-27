using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Shared.Features.Software;

namespace LogicPOS.Application.Features.Finance.Documents;

/// <summary>
/// Builds the Portuguese AT QR Code payload (fields A–R).
/// Shared by the API <c>DocumentQrCodeGenerator</c> and the certified fiscal plugin.
/// </summary>
public static class DocumentAtQrPayload
{
    /// <summary>
    /// Builds the AT QR payload string. Callers supply the four hash characters (positions 1/11/21/31).
    /// </summary>
    public static string Build(
        string? issuerFiscalNumber,
        string? customerFiscalNumber,
        string? customerCountry,
        string documentType,
        string status,
        string dateYyyyMmDd,
        string uniqueDocumentId,
        string? atcud,
        decimal totalNet,
        decimal totalTax,
        decimal totalFinal,
        string? hash4)
    {
        // Field layout matches AT QR Code technical specs (same order as the historical API generator).
        string a = "A:" + issuerFiscalNumber + "*";
        string b = "B:" + customerFiscalNumber + "*";
        string c = "C:" + customerCountry + "*";
        string d = "D:" + documentType + "*";
        string e = "E:" + status + "*";
        string f = "F:" + dateYyyyMmDd.Replace("-", "") + "*";
        string g = "G:" + uniqueDocumentId + "*";
        string h = "H:" + atcud + "*";
        string i1 = "I1:PT*";
        string i7 = "I7:" + totalNet.To2DecimalPlacesString() + "*";
        string i8 = "I8:" + totalTax.To2DecimalPlacesString() + "*";
        string n = "N:" + totalTax.To2DecimalPlacesString() + "*";
        string o = "O:" + totalFinal.To2DecimalPlacesString() + "*";
        string q = "Q:" + hash4 + "*";
        string r = "R:" + SoftwareInfo.Portugal.SaftCertificateNumber + "*";
        string s = "";

        return a + b + c + d + e + f + g + h + i1 + i7 + i8 + n + o + q + r + s;
    }
}
