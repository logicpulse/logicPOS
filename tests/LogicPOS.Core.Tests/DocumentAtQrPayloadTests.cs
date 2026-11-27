using LogicPOS.Application.Features.Finance.Documents;
using LogicPOS.Shared.Features.Software;

namespace LogicPOS.Core.Tests;

public sealed class DocumentAtQrPayloadTests
{
    private static string HashWithFourChars(char a, char b, char c, char d)
    {
        var chars = new char[40];
        Array.Fill(chars, 'x');
        chars[0] = a;
        chars[10] = b;
        chars[20] = c;
        chars[30] = d;
        return new string(chars);
    }

    [Fact]
    public void Build_matches_historical_AT_field_layout()
    {
        var hash = HashWithFourChars('Y', 'h', 'G', 'V');
        var hash4 = DocumentCertificationText.ExtractHash4Chars(hash);

        var payload = DocumentAtQrPayload.Build(
            issuerFiscalNumber: "502757191",
            customerFiscalNumber: "999999990",
            customerCountry: "PT",
            documentType: "FS",
            status: "N",
            dateYyyyMmDd: "2019-08-12",
            uniqueDocumentId: "FS CDVF/12345",
            atcud: "CDF7T5HD-12345",
            totalNet: 0.65m,
            totalTax: 0.15m,
            totalFinal: 0.80m,
            hash4: hash4);

        Assert.Equal(
            "A:502757191*B:999999990*C:PT*D:FS*E:N*F:20190812*G:FS CDVF/12345*H:CDF7T5HD-12345*I1:PT*I7:0.65*I8:0.15*N:0.15*O:0.80*Q:YhGV*R:2543*",
            payload);
        Assert.StartsWith("A:502757191*B:", payload, StringComparison.Ordinal);
        Assert.Contains("H:CDF7T5HD-12345*", payload, StringComparison.Ordinal);
        Assert.Contains("Q:YhGV*", payload, StringComparison.Ordinal);
        Assert.Contains($"R:{SoftwareInfo.Portugal.SaftCertificateNumber}*", payload, StringComparison.Ordinal);
    }

    [Fact]
    public void Certification_line_includes_hash4_and_AT_certificate_number()
    {
        var hash = HashWithFourChars('A', 'B', 'C', 'D');
        var line = DocumentCertificationText.Build(
            isPortugal: true,
            isAngola: false,
            isMozambique: false,
            documentHash: hash);

        Assert.Equal($"ABCD - Processado por programa certificado Nº {SoftwareInfo.Portugal.SaftCertificateNumber}/AT", line);
        Assert.Contains("Nº", line, StringComparison.Ordinal);
        Assert.Contains("/AT", line, StringComparison.Ordinal);
    }
}
