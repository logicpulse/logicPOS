using LogicPOS.Application.Features.System.Users;

namespace LogicPOS.Core.BackOffice;

/// <summary>
/// Maps BackOffice menu titles to the permission token that shows that item.
/// Titles without an entry stay available (system actions such as backup and logout).
/// </summary>
public static class ProfileMenuAccess
{
    private static readonly Dictionary<string, string> Tokens = new(StringComparer.Ordinal)
    {
        ["Famílias"] = UserProfilePermissions.Articles.Families.BACKOFFICE_MAN_ARTICLEFAMILY_MENU,
        ["Subfamílias"] = UserProfilePermissions.Articles.Subfamilies.BACKOFFICE_MAN_ARTICLESUBFAMILY_MENU,
        ["Artigos"] = UserProfilePermissions.Articles.BACKOFFICE_MAN_ARTICLE_MENU,
        ["Tipo de artigos"] = UserProfilePermissions.Articles.Types.BACKOFFICE_MAN_ARTICLETYPE_MENU,
        ["Classe do artigo"] = UserProfilePermissions.Articles.Classes.BACKOFFICE_MAN_ARTICLECLASS_MENU,
        ["Tipo de Preço"] = UserProfilePermissions.Articles.PriceTypes.BACKOFFICE_MAN_CONFIGURATIONPRICETYPE_MENU,
        ["Gestão de Stocks"] = UserProfilePermissions.Stocks.BACKOFFICE_MAN_ARTICLESTOCK_MENU,
        ["Importar Artigos"] = UserProfilePermissions.System.BACKOFFICE_MAN_SYSTEM_IMPORT_ARTICLES_MENU,
        ["Exportar Artigos"] = UserProfilePermissions.System.BACKOFFICE_MAN_SYSTEM_EXPORT_ARTICLES_MENU,
        ["Abertura de ano fiscal"] = UserProfilePermissions.Finance.FiscalYears.BACKOFFICE_MAN_DOCUMENTFINANCEYEARS_MENU,
        ["Séries"] = UserProfilePermissions.Finance.Documents.Series.BACKOFFICE_MAN_DOCUMENTFINANCESERIES_VIEW,
        ["Tipo de documento"] = UserProfilePermissions.Finance.Documents.Types.BACKOFFICE_MAN_DOCUMENTFINANCETYPE_MENU,
        ["Taxas de imposto"] = UserProfilePermissions.VatRates.BACKOFFICE_MAN_CONFIGURATIONVATRATE_VIEW,
        ["Motivo de isenção de IVA"] = UserProfilePermissions.VatExemptionReasons.BACKOFFICE_MAN_CONFIGURATIONVATEXEMPTIONREASON_MENU,
        ["Cond. de Pagamento"] = UserProfilePermissions.PaymentConditions.BACKOFFICE_MAN_CONFIGURATIONPAYMENTCONDITION_MENU,
        ["Métodos de pagamento"] = UserProfilePermissions.PaymentMethods.BACKOFFICE_MAN_CONFIGURATIONPAYMENTMETHOD_MENU,
        ["Clientes"] = UserProfilePermissions.Customers.BACKOFFICE_MAN_CUSTOMER_MENU,
        ["Tipo de clientes"] = UserProfilePermissions.Customers.Types.BACKOFFICE_MAN_CUSTOMERTYPE_MENU,
        ["Grupo de descontos"] = UserProfilePermissions.Customers.DiscountGroups.BACKOFFICE_MAN_CUSTOMERDISCOUNTGROUP_MENU,
        ["Importar Clientes"] = UserProfilePermissions.System.BACKOFFICE_MAN_SYSTEM_IMPORT_CUSTOMERS_MENU,
        ["Exportar Clientes"] = UserProfilePermissions.System.BACKOFFICE_MAN_SYSTEM_EXPORT_COSTUMERS_MENU,
        ["Utilizadores"] = UserProfilePermissions.System.Users.BACKOFFICE_MAN_USERDETAIL_MENU,
        ["Permissões"] = UserProfilePermissions.System.Users.BACKOFFICE_MAN_USERPERMISSIONPROFILE_MENU,
        ["Grupo de comissões"] = UserProfilePermissions.System.Users.BACKOFFICE_MAN_USERCOMMISSIONGROUP_MENU,
        ["Tipos de impressora"] = UserProfilePermissions.Printers.Types.BACKOFFICE_MAN_CONFIGURATIONPRINTERSTYPE_MENU,
        ["Impressoras"] = UserProfilePermissions.Printers.BACKOFFICE_MAN_CONFIGURATIONPRINTERS_MENU,
        ["Dispositivos de Entrada"] = UserProfilePermissions.InputReaders.BACKOFFICE_MAN_CONFIGURATIONINPUTREADER_MENU,
        ["Display de Cliente"] = UserProfilePermissions.PoleDisplays.BACKOFFICE_MAN_CONFIGURATIONPOLEDISPLAY_MENU,
        ["Balanças"] = UserProfilePermissions.WeighingMachines.BACKOFFICE_MAN_CONFIGURATIONWEIGHINGMACHINE_MENU,
        ["País"] = UserProfilePermissions.Countries.BACKOFFICE_MAN_CONFIGURATIONCOUNTRY_MENU,
        ["Moeda"] = UserProfilePermissions.Currencies.BACKOFFICE_MAN_CONFIGURATIONCURRENCY_MENU,
        ["Locais"] = UserProfilePermissions.Places.BACKOFFICE_MAN_CONFIGURATIONPLACE_MENU,
        ["Mesas"] = UserProfilePermissions.Tables.BACKOFFICE_MAN_CONFIGURATIONPLACETABLE_MENU,
        ["Tipo de Movimento"] = UserProfilePermissions.MovementTypes.BACKOFFICE_MAN_CONFIGURATIONPLACEMOVEMENTTYPE_MENU,
        ["Unidades de medida"] = UserProfilePermissions.Articles.MeasurementUnits.BACKOFFICE_MAN_CONFIGURATIONUNITMEASURE_MENU,
        ["Unidades de tamanho"] = UserProfilePermissions.Articles.SizeUnits.BACKOFFICE_MAN_CONFIGURATIONUNITSIZE_MENU,
        ["Feriados"] = UserProfilePermissions.Holidays.BACKOFFICE_MAN_CONFIGURATIONHOLIDAYS_MENU,
        ["Armazém"] = UserProfilePermissions.Stocks.Warehouses.BACKOFFICE_MAN_WAREHOUSE_MENU,
        ["Parâmetros da Empresa"] = UserProfilePermissions.PreferenceParameters.BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETERCOMPANY_MENU,
        ["Parâmetros de Sistema"] = UserProfilePermissions.PreferenceParameters.BACKOFFICE_MAN_CONFIGURATIONPREFERENCEPARAMETERSYSTEM_MENU,
        ["Terminais"] = UserProfilePermissions.Terminals.BACKOFFICE_MAN_CONFIGURATIONPLACETERMINAL_MENU,
        ["Relatórios"] = UserProfilePermissions.Reports.REPORT_ACCESS,
        ["Emissão Recibos"] = UserProfilePermissions.Finance.Documents.BACKOFFICE_MAN_DOCUMENTSPAY_MENU,
        ["Recibos"] = UserProfilePermissions.Finance.Documents.BACKOFFICE_MAN_DOCUMENTSPAYMENTS_MENU,
        ["Conta.Corr."] = UserProfilePermissions.Finance.Documents.BACKOFFICE_MAN_DOCUMENTSCURRENTACCOUNT_MENU,
        ["Documentos"] = UserProfilePermissions.Finance.Documents.BACKOFFICE_MAN_DOCUMENTSLISTALL_MENU,
        ["Sessões de Trab."] = UserProfilePermissions.WorkSessions.WORKSESSION_ALL,
        ["SAF-T ano"] = UserProfilePermissions.System.BACKOFFICE_MAN_SYSTEM_EXPORTSAFTPT_SAFTPT_MENU,
        ["SAF-T último mês"] = UserProfilePermissions.System.BACKOFFICE_MAN_SYSTEM_EXPORTSAFTPT_SAFTPT_MENU,
        ["SAF-T período"] = UserProfilePermissions.System.BACKOFFICE_MAN_SYSTEM_EXPORTSAFTPT_CUSTOM_MENU,
        ["AGT ano"] = UserProfilePermissions.System.BACKOFFICE_MAN_SYSTEM_EXPORTSAFTPT_SAFTPT_MENU,
        ["AGT último mês"] = UserProfilePermissions.System.BACKOFFICE_MAN_SYSTEM_EXPORTSAFTPT_SAFTPT_MENU,
        ["AGT período"] = UserProfilePermissions.System.BACKOFFICE_MAN_SYSTEM_EXPORTSAFTPT_CUSTOM_MENU
    };

    public static bool IsRestricted(string title) => Tokens.ContainsKey(title);

    public static IReadOnlyCollection<string> AllowedTitles(IEnumerable<string> grantedTokens)
    {
        var granted = grantedTokens as IReadOnlySet<string> ?? new HashSet<string>(grantedTokens, StringComparer.Ordinal);
        return Tokens
            .Where(pair => granted.Contains(pair.Value))
            .Select(pair => pair.Key)
            .ToArray();
    }
}
