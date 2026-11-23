namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Maps compact or legacy labels to a clearer PT-PT tooltip when the visible text stays short.
/// </summary>
public static class ListingLabelTips
{
    private static readonly Dictionary<string, string> FullTerms = new(StringComparer.Ordinal)
    {
        ["Ant."] = "Anterior",
        ["Prox."] = "Próximo",
        ["Alternar lista"] = "Alternar modo de lista",
        ["Código de barras/Artigo"] = "Código de barras ou artigo",
        ["Doc. origem"] = "Documento de origem",
        ["Doc. venda"] = "Documento de venda",
        ["Num.Doc."] = "Número do documento",
        ["Número do Doc."] = "Número do documento",
        ["Pagamento Total"] = "Pagamento total",
        ["Pagamento Parcial"] = "Pagamento parcial",
        ["Reembolso Volta"] = "Reembolso de depósito / vasilhame",
        ["Reembolso de depósito"] = "Reembolso de depósito / vasilhame (Volta)",
        ["SubT."] = "Subtotal",
        ["Código do país"] = "Código do país (2 caracteres)",
        ["Código do País (2 Caracteres)"] = "Código do país (2 caracteres)",
        ["Predefinido"] = "Terminal predefinido",
        ["Por padrão"] = "Predefinido",
        ["Cartão Crédito"] = "Cartão de Crédito",
        ["Cartão Débito"] = "Cartão de Débito",
        ["Cartão"] = "Cartão de cliente",
        ["Editar"] = "Editar",
        ["Eliminar"] = "Eliminar",
        ["Ver"] = "Ver documento",
        ["Imprimir"] = "Imprimir documento",
    };

    public static string? FullTerm(string? shortLabel)
    {
        if (string.IsNullOrWhiteSpace(shortLabel))
        {
            return null;
        }

        return FullTerms.TryGetValue(shortLabel.Trim(), out var full) ? full : null;
    }

    public static string Tooltip(string? shortLabel) => FullTerm(shortLabel) ?? shortLabel ?? string.Empty;
}
