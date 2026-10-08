namespace LogicPOS.Core.BackOffice;

public static class ListingColumns
{
    private static readonly (string Key, string Header)[] CodeDesignationUpdated =
    [
        ("Code", "Código"),
        ("Designation", "Designação"),
        ("UpdatedAt", "Atualizado em")
    ];

    private static readonly (string Key, string Header)[] CodeDesignationAcronymUpdated =
    [
        ("Code", "Código"),
        ("Designation", "Designação"),
        ("Acronym", "Acrónimo"),
        ("UpdatedAt", "Atualizado em")
    ];

    private static readonly Dictionary<string, (string Key, string Header)[]> Profiles = new(StringComparer.Ordinal)
    {
        ["Famílias"] = CodeDesignationUpdated,
        ["Tipo de Preço"] = CodeDesignationUpdated,
        ["Unidades de tamanho"] = CodeDesignationUpdated,
        ["Armazém"] = CodeDesignationUpdated,
        ["País"] = CodeDesignationUpdated,
        ["Tipo de clientes"] = CodeDesignationUpdated,
        ["Grupo de descontos"] = CodeDesignationUpdated,
        ["Tipo de Movimento"] = CodeDesignationUpdated,
        ["Balanças"] = CodeDesignationUpdated,
        ["Dispositivos de Entrada"] = CodeDesignationUpdated,
        ["Display de Cliente"] = CodeDesignationUpdated,
        ["Permissões"] = CodeDesignationUpdated,
        ["Classe do artigo"] = CodeDesignationAcronymUpdated,
        ["Unidades de medida"] = CodeDesignationAcronymUpdated,
        ["Moeda"] = CodeDesignationAcronymUpdated,
        ["Tipo de documento"] = CodeDesignationAcronymUpdated,
        ["Cond. de Pagamento"] = CodeDesignationAcronymUpdated,
        ["Métodos de pagamento"] = CodeDesignationAcronymUpdated,
        ["Motivo de isenção de IVA"] = CodeDesignationAcronymUpdated,
        ["Subfamílias"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("FamilyId", "Família do artigo"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Artigos"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("IsComposed", "Artigo composto"),
            ("SubfamilyId", "Subfamília do artigo"),
            ("TypeId", "Tipo de artigo"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Tipo de artigos"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("HasPrice", "Tem preço"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Clientes"] =
        [
            ("Code", "Código"),
            ("Name", "Clientes"),
            ("FiscalNumber", "NIF"),
            ("CardNumber", "Número do Cartão de Cliente"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Séries"] =
        [
            ("Code", "Código"),
            ("FiscalYearId", "Ano fiscal"),
            ("DocumentTypeId", "Tipo de documento"),
            ("Designation", "Designação"),
            ("TerminalId", "Posto de Trabalho"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Abertura de ano fiscal"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("Acronym", "Acrónimo"),
            ("Year", "Ano fiscal"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Taxas de imposto"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("Value", "Taxa"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Impressoras"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("PrinterTypeId", "Tipos de impressoras"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Tipos de impressora"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("ThermalPrinter", "Impressora Térmica"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Locais"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("PriceTypeId", "Tipo de preço"),
            ("MovementTypeId", "Tipo de Movimento"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Mesas"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("PlaceId", "Locais"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Terminais"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("IsDefault", "Por padrão"),
            ("HardwareId", "Hardware ID"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Feriados"] =
        [
            ("Code", "Código"),
            ("Day", "Dia"),
            ("Month", "Mês"),
            ("Year", "Ano"),
            ("Designation", "Designação"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Sessões de Trab."] =
        [
            ("Designation", "Designação"),
            ("StartDate", "Data Inicial"),
            ("EndDate", "Data Final")
        ],
        ["Parâmetros da Empresa"] =
        [
            ("ResourceString", "Designação"),
            ("Token", "Designação"),
            ("Value", "Valor"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Parâmetros de Sistema"] =
        [
            ("ResourceString", "Designação"),
            ("Token", "Designação"),
            ("Value", "Valor"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Grupo de comissões"] =
        [
            ("Code", "Código"),
            ("Designation", "Designação"),
            ("Commission", "Comissão"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Utilizadores"] =
        [
            ("Code", "Código"),
            ("Name", "Utilizadores"),
            ("ProfileId", "Perfil"),
            ("FiscalNumber", "NIF"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Gestão de Stocks"] =
        [
            ("MovementTypeId", "Movimento"),
            ("Date", "Data"),
            ("CustomerId", "Entidade"),
            ("SupplierId", "Entidade"),
            ("DocumentNumber", "Número do Doc."),
            ("ArticleId", "Artigo"),
            ("Quantity", "Quantidade"),
            ("UpdatedAt", "Atualizado em")
        ],
        ["Recibos"] =
        [
            ("Date", "Data do documento"),
            ("Number", "Número do Doc."),
            ("Status", "Estado"),
            ("CustomerId", "Entidade"),
            ("FiscalNumber", "NIF"),
            ("TotalFinal", "Total"),
            ("UpdatedAt", "Atualizado em")
        ]
    };

    public static IReadOnlyList<ListingColumn> For(string title, IEnumerable<string> available)
    {
        var keys = available.Where(key => string.IsNullOrWhiteSpace(key) == false).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        var wanted = Profiles.TryGetValue(title, out var profile) ? profile : CodeDesignationUpdated;
        var columns = new List<ListingColumn>();
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var column in wanted)
        {
            var key = keys.FirstOrDefault(item => item.Equals(column.Key, StringComparison.OrdinalIgnoreCase));
            if (key is null || used.Add(column.Header) == false)
            {
                continue;
            }

            columns.Add(new ListingColumn { Key = key, Header = column.Header, Visible = true });
        }

        if (columns.Count > 0)
        {
            return columns;
        }

        return keys.Take(6).Select(key => new ListingColumn { Key = key, Header = key, Visible = true }).ToList();
    }

    /// <summary>Flattened GTK-aligned column headers used by form labels when missing from the main map.</summary>
    public static IEnumerable<IReadOnlyDictionary<string, string>> AllHeaders()
    {
        yield return Profiles.Values
            .SelectMany(columns => columns)
            .GroupBy(column => column.Key, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First().Header, StringComparer.Ordinal);
    }
}
