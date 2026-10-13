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

    public static IReadOnlyList<ReportDefinition> All { get; } =
    [
        // Financeiros (Sumário) — GTK summary + company/customer balance
        Item("company-billing", "Faturação da empresa", GSum, "reports/company/billing/pdf", ReportFilterProfile.DateRangeOnly),
        Item("customer-balance-summary", "Saldo de clientes (sumário)", GSum, "reports/customers/current-account/summary/pdf", ReportFilterProfile.DateRangeCustomerOptional),
        Item("sales-document-type", "Vendas por tipo de documento fiscal", GSum, "reports/sales-by-document-type/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-day", "Vendas por dia", GSum, "reports/sales-by-date/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-employee", "Vendas por funcionário", GSum, "reports/sales-by-employee/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-terminal", "Vendas por terminal", GSum, "reports/sales-by-terminal/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-customer", "Vendas por cliente", GSum, "reports/sales-by-customer/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-payment-method", "Vendas por método de pagamento", GSum, "reports/sales-by-paymentmethod/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-payment-condition", "Vendas por condição de pagamento", GSum, "reports/sales-by-paymentcondition/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-currency", "Vendas por moeda", GSum, "reports/sales-by-currency/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-country", "Vendas por país", GSum, "reports/sales-by-country/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-vat-type", "Vendas por taxa IVA e tipo de artigo", GSum, "reports/sales-by-vatrate-and-articletype/pdf", ReportFilterProfile.DateRangeVat),
        Item("sales-vat-class", "Vendas por taxa IVA e classe de artigo", GSum, "reports/sales-by-vatrate-and-articleclass/pdf", ReportFilterProfile.DateRangeVat),

        // Financeiros (Detalhado)
        Item("customer-balance-details", "Saldo de clientes (detalhe)", GDet, "reports/customers/{customer}/current-account/pdf", ReportFilterProfile.DateRangeCustomerRequired),
        Item("articles-sold", "Total vendido por artigo", GDet, "reports/articles/total-sold/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-document-type-detail", "Vendas por tipo de documento fiscal (Detalhado)", GDet, "reports/sales-by-document-type/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-day-detail", "Vendas por dia (Detalhado)", GDet, "reports/sales-by-document-date/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-employee-detail", "Vendas por funcionário (Detalhado)", GDet, "reports/sales-by-employee/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-terminal-detail", "Vendas por terminal (Detalhado)", GDet, "reports/sales-by-terminal/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-customer-detail", "Vendas por cliente (Detalhado)", GDet, "reports/sales-by-customer/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-payment-method-detail", "Vendas por método de pagamento (Detalhado)", GDet, "reports/sales-by-payment-method/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-payment-condition-detail", "Vendas por condição de pagamento (Detalhado)", GDet, "reports/sales-by-payment-condition/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-currency-detail", "Vendas por moeda (Detalhado)", GDet, "reports/sales-by-currency/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-country-detail", "Vendas por país (Detalhado)", GDet, "reports/sales-by-country/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-family-detail", "Vendas por família (Detalhado)", GDet, "reports/sales-by-family/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-subfamily-detail", "Vendas por família e subfamília (Detalhado)", GDet, "reports/sales-by-subfamily/detailed/pdf", ReportFilterProfile.SubfamilyDetailed),
        Item("sales-place-detail", "Vendas por zona (Detalhado)", GDet, "reports/sales-by-place/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-table-detail", "Vendas por mesa (Detalhado)", GDet, "reports/sales-by-table/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-vat-group-detail", "Vendas por taxa IVA (Detalhado/Agrupado)", GDet, "reports/sales-by-vatrate-group/detailed/pdf", ReportFilterProfile.DateRangeVat),

        // Tabelas Auxiliares
        Item("articles", "Famílias, subfamílias e artigos", GAux, "reports/articles/pdf", ReportFilterProfile.None, dated: false),
        Item("customers", "Clientes", GAux, "reports/customers/list/pdf", ReportFilterProfile.None, dated: false),
        Item("suppliers", "Fornecedores", GAux, "reports/customers/suppliers/pdf", ReportFilterProfile.None, dated: false),

        // Outros Relatórios
        Item("audits", "Auditoria do sistema", GOth, "reports/system-audits/pdf", ReportFilterProfile.DateRangeAudit),
        Item("commissions", "Comissões de utilizadores", GOth, "reports/sales-by-commission/pdf", ReportFilterProfile.DateRangeCommissions),
        Item("deleted-orders", "Pedidos eliminados", GOth, "reports/deleted-orders/pdf", ReportFilterProfile.DateRangeOnly),

        // Stock
        Item("stock-movement", "Movimentos de stock", GStk, "reports/stock-movement/pdf", ReportFilterProfile.StockMovements),
        Item("stock", "Stock por armazém", GStk, "reports/stock/pdf", ReportFilterProfile.StockWarehouse),
        Item("stock-article", "Stock por artigo", GStk, "reports/stock-by-article/pdf", ReportFilterProfile.StockArticle, dated: false),
        Item("stock-supplier", "Stock por fornecedor", GStk, "reports/stock-by-supplier/pdf", ReportFilterProfile.StockSupplier, dated: false),
        Item("stock-gain", "Ganho por artigo", GStk, "reports/stock-by-article-gain/pdf", ReportFilterProfile.StockArticleGain, dated: false)
    ];

    private static ReportDefinition Item(
        string key,
        string name,
        string group,
        string endpoint,
        ReportFilterProfile profile,
        bool dated = true) => new()
    {
        Key = key,
        Name = name,
        Group = group,
        Endpoint = endpoint,
        Dated = dated,
        FilterProfile = profile
    };
}
