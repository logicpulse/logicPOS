namespace LogicPOS.Application.Features.Reports.Common;

internal class DocumentReportsUtils
{
    public static string GetDocumentNameFromType(string type)
    {
        return type switch
        {
            "FT" => "Fatura",
            "FR" => "Fatura Recibo",
            "FS" => "Fatura Simplificada",
            "NC" => "Nota de Crédito",
            _ => "Documento"
        };
    }
}
