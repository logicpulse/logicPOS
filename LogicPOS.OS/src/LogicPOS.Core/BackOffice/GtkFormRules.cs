using System.Text.RegularExpressions;

namespace LogicPOS.Core.BackOffice;

public static class GtkFormRules
{
    // Patterns copied from the GTK SettingsApp dialogs.
    private const string NamePattern = @"^[À-ÿ€$&@#%ºª(){}\[\]';:!?<>+\-_ \.\,\s\\\/\*\w]{1,200}$";
    private const string AddressPattern = @"^[0-9A-Za-zÀ-ÿ&ºª,;'./\\* _-]*$";
    private const string EmailPattern = @"^(|([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+([,.](([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5}){1,25})+)*$";
    private const string PhonePattern = @"^\+?([- ]?\d)*$";
    private const string FiscalPattern = @"^(PT)?[0-9]{9}$";
    private const string ZipPattern = @"^\d{4}(-\d{3})?$";
    private const string CardPattern = @"^[0-9A-Za-z]*$";
    private const string PercentPattern = @"^(100([\.\,][0]{1,})?$|[0-9]{1,2}([\.\,][0-9]{1,})?)$";
    private const string Country2Pattern = @"^(AF|AX|AL|DZ|AS|AD|AO|AI|AQ|AG|AR|AM|AW|AU|AT|AZ|BS|BH|BD|BB|BY|BE|BZ|BJ|BM|BT|BO|BQ|BA|BW|BV|BR|IO|BN|BG|BF|BI|KH|CM|CA|CV|KY|CF|TD|CL|CN|CX|CC|CO|KM|CG|CD|CK|CR|CI|HR|CU|CW|CY|CZ|DK|DJ|DM|DO|EC|EG|SV|GQ|ER|EE|ET|FK|FO|FJ|FI|FR|GF|PF|TF|GA|GM|GE|DE|GH|GI|GR|GL|GD|GP|GU|GT|GG|GN|GW|GY|HT|HM|VA|HN|HK|HU|IS|IN|ID|IR|IQ|IE|IM|IL|IT|JM|JP|JE|JO|KZ|KE|KI|KP|KR|KW|KG|LA|LV|LB|LS|LR|LY|LI|LT|LU|MO|MK|MG|MW|MY|MV|ML|MT|MH|MQ|MR|MU|YT|MX|FM|MD|MC|MN|ME|MS|MA|MZ|MM|NA|NR|NP|NL|NC|NZ|NI|NE|NG|NU|NF|MP|NO|OM|PK|PW|PS|PA|PG|PY|PE|PH|PN|PL|PT|PR|QA|RE|RO|RU|RW|BL|SH|KN|LC|MF|PM|VC|WS|SM|ST|SA|SN|RS|SC|SL|SG|SX|SK|SI|SB|SO|ZA|GS|SS|ES|LK|SD|SR|SJ|SZ|SE|CH|SY|TW|TJ|TZ|TH|TL|TG|TK|TO|TT|TN|TR|TM|TC|TV|UG|UA|AE|GB|US|UM|UY|UZ|VU|VE|VN|VG|VI|WF|EH|YE|ZM|ZW|PT-AC|PT-MA)$";
    private const string Country3Pattern = @"^(ABW|AFG|AGO|AIA|ALA|ALB|AND|ARE|ARG|ARM|ASM|ATA|ATF|ATG|AUS|AUT|AZE|BDI|BEL|BEN|BES|BFA|BGD|BGR|BHR|BHS|BIH|BLM|BLR|BLZ|BMU|BOL|BRA|BRB|BRN|BTN|BVT|BWA|CAF|CAN|CCK|CHE|CHL|CHN|CIV|CMR|COD|COG|COK|COL|COM|CPV|CRI|CUB|CUW|CXR|CYM|CYP|CZE|DEU|DJI|DMA|DNK|DOM|DZA|ECU|EGY|ERI|ESH|ESP|EST|ETH|FIN|FJI|FLK|FRA|FRO|FSM|GAB|GBR|GEO|GGY|GHA|GIB|GIN|GLP|GMB|GNB|GNQ|GRC|GRD|GRL|GTM|GUF|GUM|GUY|HKG|HMD|HND|HRV|HTI|HUN|IDN|IMN|IND|IOT|IRL|IRN|IRQ|ISL|ISR|ITA|JAM|JEY|JOR|JPN|KAZ|KEN|KGZ|KHM|KIR|KNA|KOR|KWT|LAO|LBN|LBR|LBY|LCA|LIE|LKA|LSO|LTU|LUX|LVA|MAC|MAF|MAR|MCO|MDA|MDG|MDV|MEX|MHL|MKD|MLI|MLT|MMR|MNE|MNG|MNP|MOZ|MRT|MSR|MTQ|MUS|MWI|MYS|MYT|NAM|NCL|NER|NFK|NGA|NIC|NIU|NLD|NOR|NPL|NRU|NZL|OMN|PAK|PAN|PCN|PER|PHL|PLW|PNG|POL|PRI|PRK|PRT|PRY|PSE|PYF|QAT|REU|ROU|RUS|RWA|SAU|SDN|SEN|SGP|SGS|SHN|SJM|SLB|SLE|SLV|SMR|SOM|SPM|SRB|SSD|STP|SUR|SVK|SVN|SWE|SWZ|SXM|SYC|SYR|TCA|TCD|TGO|THA|TJK|TKL|TKM|TLS|TON|TTO|TUN|TUR|TUV|TWN|TZA|UGA|UKR|UMI|URY|USA|UZB|VAT|VCT|VEN|VGB|VIR|VNM|VUT|WLF|WSM|YEM|ZAF|ZMB|ZWE|PRT-AC|PRT-MA)$";
    private const string PortPattern = @"^(COM1|COM2|COM3|COM4|COM5|COM6|COM7|COM8|COM9|COM10|COM11|COM12|COM13|COM14|COM15)$";
    private const string BaudPattern = @"^(300|600|1200|2400|4800|9600|14400|28800|36000|115000)$";
    private const string ParityPattern = @"^(None|Odd|Even|Mark|Space)$";
    private const string StopPattern = @"^(None|One|Two|OnePointFive)$";
    private const string DataBitsPattern = @"^(7|8|9)$";
    private const string DayPattern = @"^(0?[1-9]|[12][0-9]|3[01])$";
    private const string MonthPattern = @"^(0?[1-9]|1[012])$";
    private const string YearPattern = @"^((19|20)\d\d)|0$";
    private const string DecimalPattern = @"^\s*(?=.*[0-9])\d*(?:[\.,]\d{1,4})?\s*$";

    public static string? Validate(string title, IReadOnlyDictionary<string, string> fields)
    {
        return title switch
        {
            "Clientes" => Customer(fields),
            "Utilizadores" => Optional(fields, "Name", "o nome", NamePattern, required: true)
                ?? Optional(fields, "Email", "o email", EmailPattern, required: false)
                ?? Optional(fields, "Phone", "o telefone", PhonePattern, required: false)
                ?? Optional(fields, "MobilePhone", "o telemóvel", PhonePattern, required: false)
                ?? Optional(fields, "FiscalNumber", "o NIF", FiscalPattern, required: false)
                ?? Optional(fields, "ZipCode", "o código postal", ZipPattern, required: false),
            "País" => Optional(fields, "Code2", "o código de país", Country2Pattern, required: true)
                ?? Optional(fields, "Code3", "o código de país", Country3Pattern, required: false),
            "Séries" => Optional(fields, "Designation", "a designação", NamePattern, required: true),
            "Balanças" => Optional(fields, "PortName", "a porta", PortPattern, required: true)
                ?? Optional(fields, "BaudRate", "o baud", BaudPattern, required: true)
                ?? Optional(fields, "Parity", "a paridade", ParityPattern, required: true)
                ?? Optional(fields, "StopBits", "os stop bits", StopPattern, required: true)
                ?? Optional(fields, "DataBits", "os data bits", DataBitsPattern, required: true),
            "Display de Cliente" => Optional(fields, "COMPort", "a porta", PortPattern, required: false),
            "Grupo de comissões" => Optional(fields, "Commission", "a comissão", PercentPattern, required: true),
            "Feriados" => Optional(fields, "Day", "o dia", DayPattern, required: true)
                ?? Optional(fields, "Month", "o mês", MonthPattern, required: true)
                ?? Optional(fields, "Year", "o ano", YearPattern, required: true),
            "Moeda" or "Taxas de imposto" => Optional(fields, title == "Moeda" ? "ExchangeRate" : "Value", "o valor", DecimalPattern, required: true),
            _ => null
        };
    }

    private const string DatePattern = @"^(0?[1-9]|[12][0-9]|3[01])/(0?[1-9]|1[0-2])/\d{4}$";
    private const string ForeignZipPattern = @"^[0-9A-Za-z \-]{1,20}$";
    private const string ForeignFiscalPattern = @"^[0-9A-Za-z]{1,20}$";

    private static string? Customer(IReadOnlyDictionary<string, string> fields)
    {
        var portugal = Portugal(fields);
        return Optional(fields, "Name", "o nome", NamePattern, required: true)
            ?? Fiscal(fields, portugal)
            ?? RequiredLookup(fields, "CustomerTypeId", "o tipo de cliente")
            ?? RequiredLookup(fields, "PriceTypeId", "o tipo de preço")
            ?? RequiredLookup(fields, "CountryId", "o país")
            ?? Optional(fields, "Discount", "o desconto", PercentPattern, required: true)
            ?? Optional(fields, "Email", "o email", EmailPattern, required: false)
            ?? Optional(fields, "Phone", "o telefone", PhonePattern, required: false)
            ?? Optional(fields, "MobilePhone", "o telemóvel", PhonePattern, required: false)
            ?? Optional(fields, "Fax", "o fax", PhonePattern, required: false)
            ?? Optional(fields, "ZipCode", "o código postal", portugal ? ZipPattern : ForeignZipPattern, required: false)
            ?? Optional(fields, "CardNumber", "o cartão", CardPattern, required: false)
            ?? Optional(fields, "Address", "a morada", AddressPattern, required: false)
            ?? Optional(fields, "Locality", "a localidade", AddressPattern, required: false)
            ?? Optional(fields, "City", "a cidade", AddressPattern, required: false)
            ?? Optional(fields, "BirthDate", "a data de nascimento", DatePattern, required: false);
    }

    private static bool Portugal(IReadOnlyDictionary<string, string> fields)
    {
        if (fields.TryGetValue("CountryIdMeta", out var code) == false || string.IsNullOrWhiteSpace(code))
        {
            return true;
        }

        return code.Equals("PT", StringComparison.OrdinalIgnoreCase);
    }

    private static string? Fiscal(IReadOnlyDictionary<string, string> fields, bool portugal)
    {
        if (fields.TryGetValue("FiscalNumber", out var value) == false)
        {
            return null;
        }

        var text = value?.Trim() ?? string.Empty;
        if (text.Length == 0)
        {
            return "Indique o NIF.";
        }

        if (portugal)
        {
            return Regex.IsMatch(text, FiscalPattern) && PortugueseCheckDigit(text)
                ? null
                : "Indique um valor válido para o NIF.";
        }

        return Regex.IsMatch(text, ForeignFiscalPattern) ? null : "Indique um valor válido para o NIF.";
    }

    private static bool PortugueseCheckDigit(string value)
    {
        var digits = value.StartsWith("PT", StringComparison.OrdinalIgnoreCase) ? value[2..] : value;
        if (digits.Length != 9 || digits.All(char.IsDigit) == false)
        {
            return false;
        }

        var sum = 0;
        for (var index = 0; index < 8; index++)
        {
            sum += (digits[index] - '0') * (9 - index);
        }

        var check = 11 - (sum % 11);
        if (check >= 10)
        {
            check = 0;
        }

        return check == digits[8] - '0';
    }

    private static string? RequiredLookup(IReadOnlyDictionary<string, string> fields, string key, string label)
    {
        if (fields.TryGetValue(key, out var value) == false)
        {
            return null;
        }

        return string.IsNullOrWhiteSpace(value) ? $"Indique {label}." : null;
    }

    private static string? Optional(IReadOnlyDictionary<string, string> fields, string key, string label, string pattern, bool required)
    {
        if (fields.TryGetValue(key, out var value) == false)
        {
            return null;
        }

        var text = value?.Trim() ?? string.Empty;
        if (text.Length == 0)
        {
            return required ? $"Indique {label}." : null;
        }

        return Regex.IsMatch(text, pattern) ? null : $"Indique um valor válido para {label}.";
    }
}
