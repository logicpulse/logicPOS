namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Manual company marks on Parâmetros da Empresa (no turnover calculation in-app).
/// </summary>
public static class CompanyFiscalMarkTokens
{
    /// <summary>
    /// Volume de negócios acima de 100 000 € no ano anterior — obrigação de comunicar a guia à AT.
    /// </summary>
    public const string CommunicateTransportDocuments = "COMPANY_COMMUNICATE_TRANSPORT_DOCUMENTS";
}