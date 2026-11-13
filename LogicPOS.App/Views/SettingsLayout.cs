namespace LogicPOS.App.Views;

/// <summary>
/// Parâmetros da Empresa / de Sistema: tab icons, the section cards of each tab (token → card) and field icons.
/// Tokens no card claims land in an "Outros" card; tabs without cards get one card named after the tab.
/// </summary>
internal static class SettingsLayout
{
    internal sealed record Card(string Title, string Icon, int Columns, string[] Tokens);

    private const string Icons = "avares://LogicPOS.App/Assets/Images/Articles/";

    private static readonly Dictionary<string, Card[]> Cards = new(StringComparer.Ordinal)
    {
        ["Empresa"] =
        [
            new("Identificação", "tag", 3,
                ["COMPANY_NAME", "COMPANY_BUSINESS_NAME", "COMPANY_FISCALNUMBER", "COMPANY_TAX_ENTITY", "COMPANY_CAE"]),
            new("Morada", "pin", 3,
                ["COMPANY_ADDRESS", "COMPANY_CITY", "COMPANY_POSTALCODE", "COMPANY_REGION", "COMPANY_COUNTRY", "COMPANY_COUNTRY_CODE2"]),
            new("Contactos", "phone", 3,
                ["COMPANY_TELEPHONE", "COMPANY_MOBILEPHONE", "COMPANY_FAX", "COMPANY_EMAIL", "COMPANY_WEBSITE"]),
            new("Dados fiscais e registo", "coins", 3,
                ["COMPANY_STOCK_CAPITAL", "COMPANY_CIVIL_REGISTRATION", "COMPANY_CIVIL_REGISTRATION_ID", "SYSTEM_CURRENCY"]),
            new("AGT-FE", "lock", 1, ["AGT_FE_QRCODE_LOGO", AgtFePrivateKeyBox.Token])
        ],
        ["Relatórios"] =
        [
            new("Logótipos", "box", 2, ["REPORT_FILENAME_LOGO", "REPORT_FILENAME_LOGO_SMALL"]),
            new("Rodapé", "note", 2, ["REPORT_FOOTER_LINE1", "REPORT_FOOTER_LINE2"])
        ],
        ["Geral"] =
        [
            new("Documentos e talões", "printer", 2, ["USE_CC_DAILY_TICKET", "USE_POS_PDF_VIEWER", "USE_EUROPEAN_VAT_AUTOCOMPLETE"]),
            new("Divisão de clientes", "user", 3, ["SPLIT_PAYMENT_START_CLIENTS", "SPLIT_PAYMENT_MIN_CLIENTS", "SPLIT_PAYMENT_MAX_CLIENTS"]),
            new("Idioma e imagens", "gear", 2, ["CULTURE", "USE_CACHED_IMAGES"]),
            new("Dados demo", "layers", 1, ["DEMO_DATA_ENABLED"])
        ],
        ["Aparência"] =
        [
            new("Ecrã e botões", "box", 2, ["APP_SCREEN_SIZE", "APP_BUTTON_GLOSS"])
        ],
        ["SAFT"] =
        [
            new("Credenciais AT", "lock", 2, ["SERVICE_AT_PRODUCTION_ACCOUNT_FISCAL_NUMBER", "SERVICE_AT_PRODUCTION_ACCOUNT_PASSWORD"]),
            new("Comunicação à AT", "layers", 2,
            [
                "SERVICE_AT_PRODUCTION_MODE_ENABLED", "SERVICE_AT_SEND_DOCUMENTS",
                "SERVICE_AT_SEND_DOCUMENTS_WAYBILL", "SERVICE_AT_WAYBILL_AGRICULTURAL_MODE_ENABLED"
            ])
        ],
        ["Email"] =
        [
            new("Servidor", "gear", 3, ["SEND_MAIL_SMTP_SERVER", "SEND_MAIL_SMTP_PORT", "SEND_MAIL_SMTP_ENABLE_SSL"]),
            new("Autenticação", "lock", 2, ["SEND_MAIL_SMTP_USERNAME", "SEND_MAIL_SMTP_PASSWORD"]),
            new("Mensagem de documentos", "note", 1,
                ["SEND_MAIL_FINANCE_DOCUMENTS_SUBJECT", "SEND_MAIL_FINANCE_DOCUMENTS_BODY", "SEND_MAIL_FINANCE_DOCUMENTS_HTML_BODY"])
        ],
        ["Impressão"] =
        [
            new("Talão", "printer", 3,
                ["TICKET_PRINT_TICKET", "TICKET_PRINT_COMERCIAL_NAME", "TICKET_FILENAME_LOGO", "TICKET_FOOTER_LINE1", "TICKET_FOOTER_LINE2"]),
            new("Código QR", "barcode", 2, ["PRINT_QRCODE", "QRCODE_METHOD"])
        ],
        ["Backup"] =
        [
            new("Cópia automática", "calendar", 3,
            [
                "DATABASE_BACKUP_AUTOMATIC_ENABLED", "DATABASE_BACKUP_TIMESPAN",
                "DATABASE_BACKUP_TIME_SPAN_RANGE_START", "DATABASE_BACKUP_TIME_SPAN_RANGE_END"
            ])
        ],
        ["Stock"] =
        [
            new("Controlo de stock", "box", 2, ["CHECK_STOCKS", "CHECK_STOCKS_MESSAGE"])
        ],
        ["Pastas"] =
        [
            new("Pastas", "box", 2, ["PATH_BACKUPS", "PATH_SAFTPT"])
        ],
        ["Notificações"] =
        [
            new("Documentos por faturar", "note", 2, ["NOTIFICATION_DOCUMENTS_TO_INVOICE_IGNORE_AFTER_SHOW_NUMBER_OF_TIMES"])
        ],
        ["Outros"] =
        [
            new("Track API", "lock", 2, ["TRACK_API_URL", "TRACK_API_CLIENT_ID", "TRACK_API_USERNAME", "TRACK_API_PASSWORD"])
        ]
    };

    private static readonly Dictionary<string, string> FieldIcons = new(StringComparer.OrdinalIgnoreCase)
    {
        ["COMPANY_NAME"] = CustomerFieldIcons.FiscalNumber,
        ["COMPANY_BUSINESS_NAME"] = CustomerFieldIcons.PriceType,
        ["COMPANY_FISCALNUMBER"] = Icons + "icon_barcode_b.svg",
        ["COMPANY_ADDRESS"] = CustomerFieldIcons.Address,
        ["COMPANY_CITY"] = CustomerFieldIcons.City,
        ["COMPANY_POSTALCODE"] = CustomerFieldIcons.ZipCode,
        ["COMPANY_REGION"] = CustomerFieldIcons.Locality,
        ["COMPANY_COUNTRY"] = CustomerFieldIcons.Country,
        ["COMPANY_COUNTRY_CODE2"] = CustomerFieldIcons.Country,
        ["COMPANY_TELEPHONE"] = CustomerFieldIcons.Phone,
        ["COMPANY_MOBILEPHONE"] = CustomerFieldIcons.Phone,
        ["COMPANY_FAX"] = CustomerFieldIcons.Phone,
        ["COMPANY_EMAIL"] = CustomerFieldIcons.Email,
        ["COMPANY_STOCK_CAPITAL"] = CustomerFieldIcons.Discount,
        ["SYSTEM_CURRENCY"] = CustomerFieldIcons.Discount,
        ["SEND_MAIL_SMTP_USERNAME"] = CustomerFieldIcons.Name,
        ["SEND_MAIL_SMTP_PASSWORD"] = Icons + "icon_lock_b.svg",
        ["SEND_MAIL_FINANCE_DOCUMENTS_SUBJECT"] = CustomerFieldIcons.Email,
        ["SERVICE_AT_PRODUCTION_ACCOUNT_FISCAL_NUMBER"] = Icons + "icon_barcode_b.svg",
        ["SERVICE_AT_PRODUCTION_ACCOUNT_PASSWORD"] = Icons + "icon_lock_b.svg",
        ["TRACK_API_USERNAME"] = CustomerFieldIcons.Name,
        ["TRACK_API_PASSWORD"] = Icons + "icon_lock_b.svg",
        ["DATABASE_BACKUP_TIMESPAN"] = Icons + "icon_calendar_b.svg",
        ["DATABASE_BACKUP_TIME_SPAN_RANGE_START"] = Icons + "icon_calendar_b.svg",
        ["DATABASE_BACKUP_TIME_SPAN_RANGE_END"] = Icons + "icon_calendar_b.svg"
    };

    public static string TabIcon(string tab) => tab switch
    {
        "Empresa" => "tag",
        "Geral" => "gear",
        "Aparência" => "box",
        "SAFT" => "barcode",
        "Email" => "phone",
        "Impressão" => "printer",
        "Backup" => "calendar",
        "Stock" => "layers",
        "Relatórios" => "note",
        "Pastas" => "box",
        "Notificações" => "note",
        "Outros" => "gear",
        _ => "box"
    };

    public static string? FieldIcon(string token) => FieldIcons.GetValueOrDefault(token);

    /// <summary>Cards of a tab with their tokens in card order; unclaimed tokens (in their given order) form the last card.</summary>
    public static IReadOnlyList<(Card Card, List<string> Tokens)> CardsFor(string tab, IReadOnlyList<string> tokens)
    {
        var present = tokens.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var cards = Cards.GetValueOrDefault(tab) ?? [];
        var result = cards
            .Select(card => (card, card.Tokens.Where(present.Contains).ToList()))
            .ToList();
        var claimed = cards.SelectMany(card => card.Tokens).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var rest = tokens.Where(token => claimed.Contains(token) == false).ToList();
        result.Add((new Card(cards.Length == 0 ? tab : "Outros", cards.Length == 0 ? TabIcon(tab) : "gear", 3, []), rest));
        return result.Where(item => item.Item2.Count > 0).ToList();
    }
}
