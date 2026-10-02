namespace LogicPOS.Core.BackOffice;

public static class ReportCatalog
{
    public static IReadOnlyList<ReportDefinition> All { get; } =
    [
        Item("company-billing", "Relatório de Faturação da Empresa", "Financeiros (Sumário)", "reports/company/billing/pdf"),
        Item("customer-balance", "Saldo da Conta do Cliente", "Financeiros (Sumário)", "reports/customers/{customer}/current-account/pdf", customer: true),
        Item("sales-document-type", "Vendas por tipo de documento fiscal", "Financeiros (Sumário)", "reports/sales-by-document-type/pdf"),
        Item("sales-day", "Vendas por dia", "Financeiros (Sumário)", "reports/sales-by-date/pdf"),
        Item("sales-employee", "Vendas por funcionário", "Financeiros (Sumário)", "reports/sales-by-employee/pdf"),
        Item("sales-terminal", "Vendas por terminal", "Financeiros (Sumário)", "reports/sales-by-terminal/pdf"),
        Item("sales-customer", "Vendas por cliente", "Financeiros (Sumário)", "reports/sales-by-customer/pdf"),
        Item("sales-payment-method", "Vendas por métodos de pagamento", "Financeiros (Sumário)", "reports/sales-by-paymentmethod/pdf"),
        Item("sales-payment-condition", "Vendas por condições de pagamento", "Financeiros (Sumário)", "reports/sales-by-paymentcondition/pdf"),
        Item("sales-currency", "Vendas por moeda", "Financeiros (Sumário)", "reports/sales-by-currency/pdf"),
        Item("sales-country", "Vendas por país", "Financeiros (Sumário)", "reports/sales-by-country/pdf"),
        Item("sales-vat-class", "Vendas por taxa IVA / Classe de artigo", "Financeiros (Sumário)", "reports/sales-by-vatrate-and-articleclass/pdf"),
        Item("company-billing-detail", "Faturação da empresa (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-document-date/detailed/pdf"),
        Item("sales-document-type-detail", "Vendas por tipo de documento (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-document-type/detailed/pdf"),
        Item("sales-employee-detail", "Vendas por funcionário (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-employee/detailed/pdf"),
        Item("sales-terminal-detail", "Vendas por terminal (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-terminal/detailed/pdf"),
        Item("sales-customer-detail", "Vendas por cliente (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-customer/detailed/pdf"),
        Item("sales-payment-method-detail", "Vendas por método de pagamento (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-payment-method/detailed/pdf"),
        Item("sales-payment-condition-detail", "Vendas por condição de pagamento (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-payment-condition/detailed/pdf"),
        Item("sales-currency-detail", "Vendas por moeda (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-currency/detailed/pdf"),
        Item("sales-country-detail", "Vendas por país (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-country/detailed/pdf"),
        Item("sales-family-detail", "Vendas por família (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-family/detailed/pdf"),
        Item("sales-subfamily-detail", "Vendas por subfamília (detalhe)", "Financeiros (Detalhado)", "reports/sales-by-subfamily/detailed/pdf"),
        Item("articles", "Artigos", "Artigos", "reports/articles/pdf", dated: false),
        Item("articles-sold", "Total vendido por artigo", "Artigos", "reports/articles/total-sold/pdf"),
        Item("customers", "Lista de clientes", "Clientes", "reports/customers/list/pdf", dated: false),
        Item("suppliers", "Lista de fornecedores", "Clientes", "reports/customers/suppliers/pdf", dated: false),
        Item("current-account", "Conta corrente (resumo)", "Clientes", "reports/customers/current-account/summary/pdf"),
        Item("stock", "Stock por armazém", "Stock", "reports/stock/pdf"),
        Item("stock-movement", "Movimentos de stock", "Stock", "reports/stock-movement/pdf"),
        Item("stock-article", "Stock por artigo", "Stock", "reports/stock-by-article/pdf", article: true),
        Item("stock-gain", "Stock por artigo (margem)", "Stock", "reports/stock-by-article-gain/pdf"),
        Item("stock-supplier", "Stock por fornecedor", "Stock", "reports/stock-by-supplier/pdf"),
        Item("audits", "Auditoria do sistema", "Sistema", "reports/system-audits/pdf")
    ];

    private static ReportDefinition Item(
        string key,
        string name,
        string group,
        string endpoint,
        bool dated = true,
        bool customer = false,
        bool article = false) => new()
    {
        Key = key,
        Name = name,
        Group = group,
        Endpoint = endpoint,
        Dated = dated,
        NeedsCustomer = customer,
        NeedsArticle = article
    };
}
