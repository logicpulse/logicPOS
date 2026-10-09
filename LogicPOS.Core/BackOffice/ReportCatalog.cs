namespace LogicPOS.Core.BackOffice;

public static class ReportCatalog
{
    public static IReadOnlyList<ReportDefinition> All { get; } =
    [
        Item("company-billing", "Relatório de Faturação da Empresa", "Financeiros (Sumário)", "reports/company/billing/pdf", ReportFilterProfile.DateRangeOnly),
        Item("customer-balance", "Saldo da Conta do Cliente", "Financeiros (Sumário)", "reports/customers/{customer}/current-account/pdf", ReportFilterProfile.DateRangeCustomerOptional),
        Item("sales-document-type", "Vendas por tipo de documento fiscal", "Financeiros (Sumário)", "reports/sales-by-document-type/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-day", "Vendas por dia", "Financeiros (Sumário)", "reports/sales-by-date/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-employee", "Vendas por funcionário", "Financeiros (Sumário)", "reports/sales-by-employee/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-terminal", "Vendas por terminal", "Financeiros (Sumário)", "reports/sales-by-terminal/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-customer", "Vendas por cliente", "Financeiros (Sumário)", "reports/sales-by-customer/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-payment-method", "Vendas por métodos de pagamento", "Financeiros (Sumário)", "reports/sales-by-paymentmethod/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-payment-condition", "Vendas por condições de pagamento", "Financeiros (Sumário)", "reports/sales-by-paymentcondition/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-currency", "Vendas por moeda", "Financeiros (Sumário)", "reports/sales-by-currency/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-country", "Vendas por país", "Financeiros (Sumário)", "reports/sales-by-country/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-vat-class", "Vendas por taxa IVA / Classe de artigo", "Financeiros (Sumário)", "reports/sales-by-vatrate-and-articleclass/pdf", ReportFilterProfile.DateRangeVat),
        Item("company-billing-detail", "Faturação da empresa (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-document-date/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-document-type-detail", "Vendas por tipo de documento (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-document-type/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-employee-detail", "Vendas por funcionário (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-employee/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-terminal-detail", "Vendas por terminal (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-terminal/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-customer-detail", "Vendas por cliente (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-customer/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-payment-method-detail", "Vendas por método de pagamento (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-payment-method/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-payment-condition-detail", "Vendas por condição de pagamento (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-payment-condition/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-currency-detail", "Vendas por moeda (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-currency/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-country-detail", "Vendas por país (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-country/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-family-detail", "Vendas por família (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-family/detailed/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("sales-subfamily-detail", "Vendas por subfamília (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-subfamily/detailed/pdf", ReportFilterProfile.SubfamilyDetailed),
        Item("articles", "Artigos", "Artigos", "reports/articles/pdf", ReportFilterProfile.None, dated: false),
        Item("articles-sold", "Total vendido por artigo", "Artigos", "reports/articles/total-sold/pdf", ReportFilterProfile.DateRangeDocTerminal),
        Item("customers", "Lista de clientes", "Clientes", "reports/customers/list/pdf", ReportFilterProfile.None, dated: false),
        Item("suppliers", "Lista de fornecedores", "Clientes", "reports/customers/suppliers/pdf", ReportFilterProfile.None, dated: false),
        Item("current-account", "Conta corrente (resumo)", "Clientes", "reports/customers/current-account/summary/pdf", ReportFilterProfile.DateRangeCustomerOptional),
        Item("stock", "Stock por armazém", "Stock", "reports/stock/pdf", ReportFilterProfile.StockWarehouse),
        Item("stock-movement", "Movimentos de stock", "Stock", "reports/stock-movement/pdf", ReportFilterProfile.StockMovements),
        Item("stock-article", "Stock por artigo", "Stock", "reports/stock-by-article/pdf", ReportFilterProfile.StockArticle, dated: false),
        Item("stock-gain", "Stock por artigo (margem)", "Stock", "reports/stock-by-article-gain/pdf", ReportFilterProfile.StockArticleGain, dated: false),
        Item("stock-supplier", "Stock por fornecedor", "Stock", "reports/stock-by-supplier/pdf", ReportFilterProfile.StockSupplier, dated: false),
        Item("audits", "Auditoria do sistema", "Sistema", "reports/system-audits/pdf", ReportFilterProfile.DateRangeAudit)
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
