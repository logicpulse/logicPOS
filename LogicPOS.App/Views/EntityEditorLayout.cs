using LogicPOS.Core.BackOffice;

namespace LogicPOS.App.Views;

/// <summary>
/// BackOffice entity editors: dialog titles, tab icons and the section cards of each tab, per listing.
/// Fields no card claims land in the tab's first card; listings or tabs without cards get one card.
/// </summary>
internal static class EntityEditorLayout
{
    /// <summary>Profile permissions share one tab; their permission groups become cards.</summary>
    public const string PermissionsTab = "Permissões";

    internal sealed record Card(string Title, string Icon, int Columns, string[] Keys);

    private static readonly Card Notes = new("Observações", "note", 1, ["Notes"]);

    private static Card Identity(int columns, params string[] keys) => new("Identificação", "tag", columns, keys);

    private static readonly Card BasicIdentity = Identity(3, "Designation", "Code", "Order");

    /// <summary>Detalhes and Detalhes financeiros keep their bespoke layout in EntityListing.</summary>
    private static readonly Dictionary<string, Card[]> ArticleCards = new(StringComparer.Ordinal)
    {
        ["Artigos Associados"] = [new("Artigos Associados", "layers", 2, [])]
    };

    private static readonly Dictionary<string, Card[]> ParameterCards = new(StringComparer.Ordinal)
    {
        [""] =
        [
            new("Parâmetro", "gear", 2, ["Token", "ResourceString", "ValueTip", "Value"]),
            new("Configuração", "layers", 3, ["InputType", "FormType", "FormPageNo", "RegEx", "ResourceStringInfo", "Code", "Order", "Required"]),
            Notes
        ]
    };

    private static readonly Dictionary<string, (string Noun, bool Feminine)> Nouns = new(StringComparer.Ordinal)
    {
        ["Famílias"] = ("família", true),
        ["Subfamílias"] = ("subfamília", true),
        ["Artigos"] = ("artigo", false),
        ["Articles"] = ("artigo", false),
        ["Tipo de artigos"] = ("tipo de artigo", false),
        ["Classe do artigo"] = ("classe de artigo", true),
        ["Tipo de Preço"] = ("tipo de preço", false),
        ["Abertura de ano fiscal"] = ("ano fiscal", false),
        ["Séries"] = ("série", true),
        ["Tipo de documento"] = ("tipo de documento", false),
        ["Taxas de imposto"] = ("taxa de imposto", true),
        ["Motivo de isenção de IVA"] = ("motivo de isenção", false),
        ["Cond. de Pagamento"] = ("condição de pagamento", true),
        ["Métodos de pagamento"] = ("método de pagamento", false),
        ["Clientes"] = ("cliente", false),
        ["Tipo de clientes"] = ("tipo de cliente", false),
        ["Grupo de descontos"] = ("grupo de descontos", false),
        ["Utilizadores"] = ("utilizador", false),
        ["Permissões"] = ("perfil", false),
        ["Grupo de comissões"] = ("grupo de comissões", false),
        ["Tipos de impressora"] = ("tipo de impressora", false),
        ["Impressoras"] = ("impressora", true),
        ["Dispositivos de Entrada"] = ("dispositivo de entrada", false),
        ["Display de Cliente"] = ("display de cliente", false),
        ["Balanças"] = ("balança", true),
        ["País"] = ("país", false),
        ["Moeda"] = ("moeda", true),
        ["Locais"] = ("local", false),
        ["Mesas"] = ("mesa", true),
        ["Tipo de Movimento"] = ("tipo de movimento", false),
        ["Unidades de medida"] = ("unidade de medida", true),
        ["Unidades de tamanho"] = ("unidade de tamanho", true),
        ["Feriados"] = ("feriado", false),
        ["Armazém"] = ("armazém", false),
        ["Parâmetros da Empresa"] = ("parâmetro", false),
        ["Parâmetros de Sistema"] = ("parâmetro", false),
        ["Terminais"] = ("terminal", false),
        ["Notificações"] = ("notificação", true),
        ["Sessões de Trab."] = ("sessão de trabalho", true)
    };

    /// <summary>Cards per listing, then per tab ("" for single-tab editors).</summary>
    private static readonly Dictionary<string, Dictionary<string, Card[]>> Cards = new(StringComparer.Ordinal)
    {
        ["Famílias"] = new()
        {
            [""] =
            [
                BasicIdentity,
                new("Classificação", "layers", 2, ["CommissionGroupId", "DiscountGroupId"]),
                new("Botão", "box", 1, ["ButtonLabel", "ButtonImage"]),
                Notes
            ]
        },
        ["Subfamílias"] = new()
        {
            [""] =
            [
                BasicIdentity,
                new("Classificação", "layers", 3, ["FamilyId", "CommissionGroupId", "DiscountGroupId"]),
                new("Impostos", "coins", 2, ["VatOnTableId", "VatDirectSellingId"]),
                new("Botão", "box", 1, ["ButtonLabel", "ButtonImage"]),
                Notes
            ]
        },
        ["Artigos"] = ArticleCards,
        ["Articles"] = ArticleCards,
        ["Clientes"] = new()
        {
            ["Geral"] =
            [
                new("Identificação", "user", 3, ["Name", "FiscalNumber", "Code", "CustomerTypeId", "BirthDate", "Order", "Supplier"]),
                Notes
            ],
            ["Morada e contactos"] =
            [
                new("Morada", "pin", 3, ["Address", "Locality", "ZipCode", "City", "CountryId"]),
                new("Contactos", "phone", 3, ["Phone", "MobilePhone", "Fax", "Email", "WebSite"])
            ],
            ["Comercial"] =
            [
                new("Preços e descontos", "coins", 2, ["PriceTypeId", "DiscountGroupId", "DiscountType", "Discount"]),
                new("Cartão de cliente", "card", 3, ["CardNumber", "CardMode", "CardCredit"])
            ]
        },
        ["Utilizadores"] = new()
        {
            ["Detalhe"] =
            [
                new("Identificação", "user", 2, ["Name", "ProfileId", "CommissionGroupId", "DateOfContract"]),
                new("Acesso", "lock", 2, ["Login", "Pin", "PortalPassword", "PortalPasswordConfirm", "PasswordReset"])
            ],
            ["Contactos"] =
            [
                new("Morada", "pin", 2, ["Residence", "Locality", "ZipCpde", "City"]),
                new("Contactos", "phone", 3, ["Phone", "MobilePhone", "Email"])
            ],
            ["Outros"] =
            [
                new("Dados adicionais", "gear", 3, ["Code", "Order", "FiscalNumber", "Language", "AccessCardNumber", "AssignedSeating", "PasswordResetDate", "Need2FA"]),
                Notes
            ]
        },
        ["Permissões"] = new()
        {
            ["Perfil"] = [new("Perfil", "user", 3, ["Designation", "Code", "Order"]), Notes]
        },
        ["Abertura de ano fiscal"] = new()
        {
            [""] =
            [
                BasicIdentity,
                new("Ano fiscal", "calendar", 2, ["Year", "Acronym", "SeriesForEachTerminal"]),
                Notes
            ]
        },
        ["Séries"] = new()
        {
            [""] =
            [
                Identity(2, "Designation", "Acronym", "Code", "Order"),
                new("Documento", "layers", 3, ["DocumentTypeId", "FiscalYearId", "TerminalId"]),
                new("Numeração", "barcode", 3, ["NextNumber", "NumberRangeBegin", "NumberRangeEnd", "ATDocCodeValidationSeries"]),
                Notes
            ]
        },
        ["Tipo de documento"] = new()
        {
            [""] =
            [
                Identity(2, "Designation", "Acronym", "Code", "Order"),
                new("Impressão", "printer", 2, ["PrintCopies", "PrintRequestMotive", "PrintRequestConfirmation", "PrintOpenDrawer"]),
                new("Fiscal e stock", "coins", 2, ["SaftDocumentType", "StockMode", "CreditDebit", "AcronymLastSerie", "Credit", "WayBill", "WsAtDocument", "SaftAuditFile"]),
                Notes
            ]
        },
        ["Taxas de imposto"] = new()
        {
            [""] =
            [
                Identity(2, "Designation", "Description", "Code", "Order"),
                new("Taxa", "coins", 3, ["Value", "TaxType", "TaxCode", "CountryRegion", "ReasonCode", "ExpirationDate"]),
                Notes
            ]
        },
        ["Métodos de pagamento"] = new()
        {
            [""] =
            [
                Identity(2, "Designation", "Acronym", "Code", "Order"),
                new("Configuração", "gear", 3, ["Token", "ResourceString", "Symbol", "ButtonIcon", "AllowPayback"]),
                Notes
            ]
        },
        ["Impressoras"] = new()
        {
            [""] =
            [
                Identity(2, "Designation", "TypeId", "Code", "Order"),
                new("Ligação", "printer", 2, ["NetworkName", "PaperWidth", "ShowInDialog"]),
                new("Impressão térmica", "gear", 3,
                [
                    "ThermalEncoding", "ThermalCutCommand", "ThermalImageCompanyLogo",
                    "ThermalOpenDrawerValueM", "ThermalOpenDrawerValueT1", "ThermalOpenDrawerValueT2", "ThermalPrintLogo"
                ]),
                Notes
            ]
        },
        ["Display de Cliente"] = new()
        {
            [""] =
            [
                BasicIdentity,
                new("Ligação", "gear", 3, ["VendorId", "ProductId", "EndPoint", "COMPort", "CodeTable"]),
                new("Ecrã", "box", 2, ["CharactersPerLine", "GoToStandByInSeconds", "StandByLine1", "StandByLine2"]),
                Notes
            ]
        },
        ["Balanças"] = new()
        {
            [""] =
            [
                BasicIdentity,
                new("Porta série", "gear", 3, ["PortName", "BaudRate", "Parity", "DataBits", "StopBits"]),
                Notes
            ]
        },
        ["País"] = new()
        {
            [""] =
            [
                BasicIdentity,
                new("Dados do país", "pin", 3, ["Code2", "Code3", "TLD", "Capital", "Currency", "CurrencyCode"]),
                new("Validação", "gear", 2, ["FiscalNumberRegex", "ZipCodeRegex"]),
                Notes
            ]
        },
        ["Moeda"] = new()
        {
            [""] =
            [
                BasicIdentity,
                new("Moeda", "coins", 2, ["Acronym", "Symbol", "Entity", "ExchangeRate"]),
                Notes
            ]
        },
        ["Mesas"] = new()
        {
            [""] =
            [
                Identity(2, "Designation", "PlaceId", "Code", "Order"),
                new("Estado", "gear", 2, ["Status", "Discount", "OpennedAt", "ClosedAt"]),
                Notes
            ]
        },
        ["Feriados"] = new()
        {
            [""] =
            [
                Identity(2, "Designation", "Description", "Code", "Order"),
                new("Data", "calendar", 3, ["Day", "Month", "Year", "Fixed"]),
                Notes
            ]
        },
        ["Parâmetros da Empresa"] = ParameterCards,
        ["Parâmetros de Sistema"] = ParameterCards,
        ["Terminais"] = new()
        {
            [""] =
            [
                BasicIdentity,
                new("Posto", "pin", 3, ["HardwareId", "PlaceId", "TimerInterval", "IsDefault"]),
                new("Periféricos", "printer", 3, ["PrinterId", "ThermalPrinterId", "WeighingMachineId", "BarcodeReaderId", "CardReaderId", "PoleDisplayId"]),
                Notes
            ]
        }
    };

    public static string Title(string listing, bool creating)
    {
        if (Nouns.TryGetValue(listing, out var noun) == false)
        {
            return creating ? "Novo" : "Editar";
        }

        return creating ? $"{(noun.Feminine ? "Nova" : "Novo")} {noun.Noun}" : $"Editar {noun.Noun}";
    }

    public static string TabIcon(string group) => group switch
    {
        "Detalhes" => "tag",
        "Detalhes financeiros" => "coins",
        "Artigos Associados" => "layers",
        "Número de série" => "barcode",
        "Geral" or "Detalhe" or "Perfil" => "user",
        "Morada e contactos" => "pin",
        "Comercial" => "coins",
        "Contactos" => "phone",
        "Outros" => "gear",
        PermissionsTab => "lock",
        _ => "box"
    };

    /// <summary>Editor tab of a field: profile permissions share one tab whatever their permission group.</summary>
    public static string? TabOf(ListingField field)
        => field.Key.StartsWith("perm:", StringComparison.Ordinal) ? PermissionsTab : field.Group;

    public static IReadOnlyList<Card> CardsFor(string listing, string? group, IReadOnlyList<ListingField> fields)
    {
        if (Cards.TryGetValue(listing, out var tabs) && tabs.TryGetValue(group ?? string.Empty, out var cards))
        {
            return cards;
        }

        var gridFields = fields.Count(field => field.Kind is not ("bool" or "multiline" or "image"));
        return [new(group ?? "Geral", group is null ? "tag" : TabIcon(group), gridFields <= 4 ? 2 : 3, [])];
    }
}
