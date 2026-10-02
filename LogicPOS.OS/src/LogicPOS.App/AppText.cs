namespace LogicPOS.App;

internal static class AppText
{
    private static readonly Dictionary<string, Dictionary<string, string>> Tables = new(StringComparer.Ordinal)
    {
        ["en"] = new(StringComparer.Ordinal)
        {
            ["Documentos"] = "Documents",
            ["Relatórios"] = "Reports",
            ["Artigos"] = "Articles",
            ["Informação fiscal"] = "Tax",
            ["Clientes"] = "Customers",
            ["Utilizadores"] = "Users",
            ["Dispositivos"] = "Devices",
            ["Outras Tabelas"] = "Other tables",
            ["Configuração"] = "Settings",
            ["Importar"] = "Import",
            ["Exportar"] = "Export",
            ["Sistema"] = "System",
            ["Dashboard"] = "Dashboard",
            ["FrontOffice"] = "Front office",
            ["Sair da Aplicação"] = "Exit",
            ["Novo Doc."] = "New document",
            ["Emissão Recibos"] = "Issue receipts",
            ["Recibos"] = "Receipts",
            ["Conta.Corr."] = "Current account",
            ["Sessões de Trab."] = "Work sessions",
            ["Famílias"] = "Families",
            ["Subfamílias"] = "Subfamilies",
            ["Tipo de artigos"] = "Article types",
            ["Classe do artigo"] = "Article class",
            ["Tipo de Preço"] = "Price type",
            ["Gestão de Stocks"] = "Stock",
            ["Abertura de ano fiscal"] = "Fiscal year",
            ["Séries"] = "Series",
            ["Tipo de documento"] = "Document type",
            ["Taxas de imposto"] = "VAT rates",
            ["Motivo de isenção de IVA"] = "VAT exemption",
            ["Cond. de Pagamento"] = "Payment terms",
            ["Métodos de pagamento"] = "Payment methods",
            ["Tipo de clientes"] = "Customer types",
            ["Grupo de descontos"] = "Discount groups",
            ["Permissões"] = "Permissions",
            ["Grupo de comissões"] = "Commission groups",
            ["Tipos de impressora"] = "Printer types",
            ["Impressoras"] = "Printers",
            ["Dispositivos de Entrada"] = "Input devices",
            ["Display de Cliente"] = "Customer display",
            ["Balanças"] = "Scales",
            ["País"] = "Country",
            ["Moeda"] = "Currency",
            ["Locais"] = "Places",
            ["Mesas"] = "Tables",
            ["Tipo de Movimento"] = "Movement type",
            ["Unidades de medida"] = "Units",
            ["Unidades de tamanho"] = "Size units",
            ["Feriados"] = "Holidays",
            ["Armazém"] = "Warehouse",
            ["Parâmetros da Empresa"] = "Company settings",
            ["Parâmetros de Sistema"] = "System settings",
            ["Terminais"] = "Terminals",
            ["Importar Artigos"] = "Import articles",
            ["Importar Clientes"] = "Import customers",
            ["Exportar Artigos"] = "Export articles",
            ["Exportar Clientes"] = "Export customers",
            ["Notificações"] = "Notifications",
            ["Registro de alterações (Changelog)"] = "Changelog",
            ["Backup DB"] = "Backup",
            ["Restaurar DB"] = "Restore",
            ["Sair da Sessão"] = "Sign out"
        },
        ["fr"] = new(StringComparer.Ordinal)
        {
            ["Documentos"] = "Documents",
            ["Relatórios"] = "Rapports",
            ["Artigos"] = "Articles",
            ["Clientes"] = "Clients",
            ["Utilizadores"] = "Utilisateurs",
            ["Configuração"] = "Configuration",
            ["Sistema"] = "Système",
            ["Sair da Aplicação"] = "Quitter",
            ["FrontOffice"] = "Caisse"
        },
        ["es"] = new(StringComparer.Ordinal)
        {
            ["Documentos"] = "Documentos",
            ["Relatórios"] = "Informes",
            ["Artigos"] = "Artículos",
            ["Clientes"] = "Clientes",
            ["Utilizadores"] = "Usuarios",
            ["Configuração"] = "Configuración",
            ["Sistema"] = "Sistema",
            ["Sair da Aplicação"] = "Salir",
            ["FrontOffice"] = "Punto de venta"
        }
    };

    public static string Get(string key)
    {
        var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        if (language == "pt" || Tables.TryGetValue(language, out var table) == false)
        {
            return key;
        }

        return table.TryGetValue(key, out var value) ? value : key;
    }
}
