using LogicPOS.Shared.Features.Software;

namespace LogicPOS.Application.Features.Finance.Documents;

/// <summary>
/// Builds the fiscal software-certification line for PDF and thermal printing.
/// Hash characters use the AT/AGT positions (1, 11, 21, 31), same as document signing.
/// </summary>
public static class DocumentCertificationText
{
    public static string Build(
        bool isPortugal,
        bool isAngola,
        bool isMozambique,
        string? documentHash,
        bool isPayment = false)
    {
        var body = BuildBody(isPortugal, isAngola, isMozambique, isPayment);
        var hash4 = ExtractHash4Chars(documentHash);
        if (string.IsNullOrEmpty(hash4) || string.IsNullOrEmpty(body))
        {
            return body;
        }

        // Portugal and Angola prefix the validation hash on finance documents (legacy thermal/PDF).
        if (isPortugal || isAngola)
        {
            return $"{hash4} - {body}";
        }

        return body;
    }

    private static string BuildBody(bool isPortugal, bool isAngola, bool isMozambique, bool isPayment)
    {
        if (isPortugal)
        {
            var prefix = isPayment ? "Emitido" : "Processado";
            return $"{prefix} por programa certificado Nº {SoftwareInfo.Portugal.SaftCertificateNumber}/AT";
        }

        if (isAngola)
        {
            return
                $"Processado por programa validado Nº {SoftwareInfo.Angola.AgtFeSoftwareValidationNumber}";
        }

        if (isMozambique)
        {
            return "Autorização da AT: DAFM1 - 0198/2018\nProcessado";
        }

        return "Processado por computador";
    }

    /// <summary>Same positions as <c>IDocumentHasher.GenerateHash4Chars</c>.</summary>
    public static string ExtractHash4Chars(string? hash)
    {
        if (string.IsNullOrEmpty(hash) || hash.Length < 31)
        {
            return string.Empty;
        }

        return $"{hash[0]}{hash[10]}{hash[20]}{hash[30]}";
    }
}
