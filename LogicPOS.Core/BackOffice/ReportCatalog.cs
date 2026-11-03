namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Avalonia report menu — same groups/items as GTK ReportsTypeToken / web ReportMenuCatalog,
/// wired to API QuestPDF endpoints.
/// </summary>
public static class ReportCatalog
{
    private const string GSum = "Financeiros (Sumário)";
    private const string GDet = "Financeiros (Detalhado)";
    private const string GAux = "Tabelas Auxiliares";
    private const string GOth = "Outros Relatórios";
    private const string GStk = "Stock";

    private const string IconSum = "Menu/menu_relatorios_financeiro_sumario.svg";
    private const string IconDet = "Menu/menu_relatorios_financeiro_detalhado.svg";
    private const string IconAux = "Menu/menu_relatorios_tabelas_auxiliares.svg";
    private const string IconOth = "Menu/menu_relatorios_outros_relatorios.svg";
    private const string IconStk = "Menu/menu_relatorios_stock.svg";

    public static IReadOnlyList<ReportDefinition> All { get; } =
    [
        // Financeiros (Sumário) — GTK summary + company/customer balance
        Item("company-billing", "Faturação da empresa", GSum, "reports/company/billing/pdf", ReportFilterProfile.DateRangeOnly, "Menu/menu_relatorios_faturacao_empresa.svg"),
        Item("customer-balance-summary", "Saldo de clientes (sumário)", GSum, "reports/customers/current-account/summary/pdf", ReportFilterProfile.DateRangeCustomerOptional, "Menu/menu_relatorios_saldo_clientes.svg"),
        Item("sales-document-type", "Vendas por tipo de documento fiscal", GSum, "reports/sales-by-document-type/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_documento_fiscal.svg"),
        Item("sales-day", "Vendas por dia", GSum, "reports/sales-by-date/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_dia.svg"),
        Item("sales-employee", "Vendas por funcionário", GSum, "reports/sales-by-employee/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_funcionarios.svg"),
        Item("sales-terminal", "Vendas por terminal", GSum, "reports/sales-by-terminal/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_venda_por_terminal.svg"),
        Item("sales-customer", "Vendas por cliente", GSum, "reports/sales-by-customer/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_cliente.svg"),
        Item("sales-payment-method", "Vendas por método de pagamento", GSum, "reports/sales-by-paymentmethod/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_metodo_pagamento.svg"),
        Item("sales-payment-condition", "Vendas por condição de pagamento", GSum, "reports/sales-by-paymentcondition/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_condicao_pagamento.svg"),
        Item("sales-currency", "Vendas por moeda", GSum, "reports/sales-by-currency/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_moeda.svg"),
        Item("sales-country", "Vendas por país", GSum, "reports/sales-by-country/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_pais.svg"),
        Item("sales-vat-type", "Vendas por taxa IVA e tipo de artigo", GSum, "reports/sales-by-vatrate-and-articletype/pdf", ReportFilterProfile.DateRangeVat, "Menu/menu_relatorios_vendas_taxa_iva_tipo_artigo.svg"),
        Item("sales-vat-class", "Vendas por taxa IVA e classe de artigo", GSum, "reports/sales-by-vatrate-and-articleclass/pdf", ReportFilterProfile.DateRangeVat, "Menu/menu_relatorios_vendas_por_taxa_iva_classe_artigo.svg"),

        // Financeiros (Detalhado)
        Item("customer-balance-details", "Saldo de clientes (detalhe)", GDet, "reports/customers/{customer}/current-account/pdf", ReportFilterProfile.DateRangeCustomerRequired, "Menu/menu_relatorios_saldo_clientes_detalhado.svg"),
        Item("articles-sold", "Total vendido por artigo", GDet, "reports/articles/total-sold/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_total_vendidos_por_artigo.svg"),
        Item("sales-document-type-detail", "Vendas por tipo de documento fiscal (Detalhado)", GDet, "reports/sales-by-document-type/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_tipo_documento_fiscal_detalhado.svg"),
        Item("sales-day-detail", "Vendas por dia (Detalhado)", GDet, "reports/sales-by-document-date/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_dia_detalhado.svg"),
        Item("sales-employee-detail", "Vendas por funcionário (Detalhado)", GDet, "reports/sales-by-employee/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_funcionario_detalhado.svg"),
        Item("sales-terminal-detail", "Vendas por terminal (Detalhado)", GDet, "reports/sales-by-terminal/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_terminal_detalhado.svg"),
        Item("sales-customer-detail", "Vendas por cliente (Detalhado)", GDet, "reports/sales-by-customer/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_cliente_detalhado.svg"),
        Item("sales-payment-method-detail", "Vendas por método de pagamento (Detalhado)", GDet, "reports/sales-by-payment-method/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_metodo_pagamento_detalhado.svg"),
        Item("sales-payment-condition-detail", "Vendas por condição de pagamento (Detalhado)", GDet, "reports/sales-by-payment-condition/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_condicao_pagamento_detalhado.svg"),
        Item("sales-currency-detail", "Vendas por moeda (Detalhado)", GDet, "reports/sales-by-currency/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_moedas_detalhado.svg"),
        Item("sales-country-detail", "Vendas por país (Detalhado)", GDet, "reports/sales-by-country/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_pais_detalhado.svg"),
        Item("sales-family-detail", "Vendas por família (Detalhado)", GDet, "reports/sales-by-family/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_artigos_familas.svg"),
        Item("sales-subfamily-detail", "Vendas por família e subfamília (Detalhado)", GDet, "reports/sales-by-subfamily/detailed/pdf", ReportFilterProfile.SubfamilyDetailed, "Menu/menu_relatorios_vendas_familia_subfamilia_detalhado.svg"),
        Item("sales-place-detail", "Vendas por zona (Detalhado)", GDet, "reports/sales-by-place/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_zona_detalhado.svg"),
        Item("sales-table-detail", "Vendas por mesa (Detalhado)", GDet, "reports/sales-by-table/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal, "Menu/menu_relatorios_vendas_por_mesa_detalhado.svg"),
        Item("sales-vat-group-detail", "Vendas por taxa IVA (Detalhado/Agrupado)", GDet, "reports/sales-by-vatrate-group/detailed/pdf", ReportFilterProfile.DateRangeVat, "Menu/menu_relatorios_vendas_taxa_iva_tipo_artigo_detalhado.svg"),

        // Tabelas Auxiliares
        Item("articles", "Famílias, subfamílias e artigos", GAux, "reports/articles/pdf", ReportFilterProfile.None, "Menu/menu_relatorios_tabelas_auxiliares_relatorios_familias_subfamilias_artigos.svg", dated: false),
        Item("customers", "Clientes", GAux, "reports/customers/list/pdf", ReportFilterProfile.None, "Menu/menu_relatorios_tabelas_auxiliares_relatorios_clientes.svg", dated: false),
        Item("suppliers", "Fornecedores", GAux, "reports/customers/suppliers/pdf", ReportFilterProfile.None, "Menu/menu_relatorios_tabelas_auxiliares_fornecedores.svg", dated: false),

        // Outros Relatórios
        Item("audits", "Auditoria do sistema", GOth, "reports/system-audits/pdf", ReportFilterProfile.DateRangeAudit, "Menu/menu_outros_relatorios_auditoria_sistema.svg"),
        Item("commissions", "Comissões de utilizadores", GOth, "reports/sales-by-commission/pdf", ReportFilterProfile.DateRangeCommissions, "Menu/menu_utilizadores_grupo_comicao.svg"),
        Item("deleted-orders", "Pedidos eliminados", GOth, "reports/deleted-orders/pdf", ReportFilterProfile.DateRangeOnly, "Menu/menu_outros_relatorios_pedidos_eliminados.svg"),

        // Stock
        Item("stock-movement", "Movimentos de stock", GStk, "reports/stock-movement/pdf", ReportFilterProfile.StockMovements, "Menu/menu_relatorios_stock_movimento_stock.svg"),
        Item("stock", "Stock por armazém", GStk, "reports/stock/pdf", ReportFilterProfile.StockWarehouse, "Menu/menu_relatorios_stock_por_armazem.svg"),
        Item("stock-article", "Stock por artigo", GStk, "reports/stock-by-article/pdf", ReportFilterProfile.StockArticle, "Menu/menu_relatorios_stock_por_artigo.svg", dated: false),
        Item("stock-supplier", "Stock por fornecedor", GStk, "reports/stock-by-supplier/pdf", ReportFilterProfile.StockSupplier, "Menu/menu_relatorios_stock_por_fornecedor.svg", dated: false),
        Item("stock-gain", "Ganho por artigo", GStk, "reports/stock-by-article-gain/pdf", ReportFilterProfile.StockArticleGain, "Menu/menu_relatorios_stock_ganho_por_artigo.svg", dated: false)
    ];

    private static ReportDefinition Item(
        string key,
        string name,
        string group,
        string endpoint,
        ReportFilterProfile profile,
        string? icon = null,
        bool dated = true) => new()
    {
        Key = key,
        Name = name,
        Group = group,
        Endpoint = endpoint,
        Dated = dated,
        FilterProfile = profile,
        Icon = icon ?? GroupIcon(group)
    };

    private static string GroupIcon(string group) => group switch
    {
        GSum => IconSum,
        GDet => IconDet,
        GAux => IconAux,
        GOth => IconOth,
        GStk => IconStk,
        _ => "Menu/menu_relatorios_relatorios.svg"
    };
}
