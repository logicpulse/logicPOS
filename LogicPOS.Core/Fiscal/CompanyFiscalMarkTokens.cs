namespace LogicPOS.Core.Fiscal;

/// <summary>
/// Manual company marks on Parâmetros da Empresa.
/// </summary>
public static class CompanyFiscalMarkTokens
{
    /// <summary>Regime de isenção do artigo 53.º do CIVA — fatura simplificada sem teto de valor.</summary>
    public const string RegimeArtigo53 = "COMPANY_REGIME_ARTIGO_53";

    /// <summary>Vendedor ambulante — caminho dos 1000 € fora dos modos de serviço.</summary>
    public const string VendedorAmbulante = "COMPANY_VENDEDOR_AMBULANTE";
}