namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Short visible labels stay compact; tooltips carry the full PT-PT term (manual §4 / triagem 2.0.31).
/// </summary>
public static class ListingLabelTips
{
    private static readonly Dictionary<string, string> FullTerms = new(StringComparer.Ordinal)
    {
        ["Ant."] = "Anterior",
        ["Prox."] = "Próximo",
        ["Alt. lista"] = "Alternar modo de lista",
        ["Cód.Barras/Artigo"] = "Código de barras ou artigo",
        ["Cond. de Pagamento"] = "Condições de Pagamento",
        ["Conta.Corr."] = "Conta corrente",
        ["Doc. origem"] = "Documento de origem",
        ["Doc. venda"] = "Documento de venda",
        ["Fin.pedido"] = "Finalizar pedido",
        ["Modif. Loc."] = "Modificar localização",
        ["Num.Doc."] = "Número do documento",
        ["Número do Doc."] = "Número do documento",
        ["Pag.Parcial"] = "Pagamento parcial",
        ["Pag.Total"] = "Pagamento total",
        ["Reemb. Volta"] = "Reembolso de volta",
        ["Sessões de Trab."] = "Sessões de Trabalho",
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