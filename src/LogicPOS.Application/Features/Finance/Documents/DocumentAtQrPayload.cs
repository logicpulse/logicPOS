using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Shared.Features.Software;

namespace LogicPOS.Application.Features.Finance.Documents;

/// <summary>
/// One VAT bucket for AT QR fields I/J/K (SAF-T TaxCountryRegion + TaxCode).
/// </summary>
public readonly record struct AtQrTaxBucket(
    string CountryRegion,
    string TaxCode,
    decimal TaxableBase,
    decimal TaxAmount);

/// <summary>
/// Aggregated tax spaces (I/J/K) plus non-subject amount for field L.
/// Amounts are in document currency; <see cref="DocumentAtQrPayload.Build"/> converts to EUR.
/// </summary>
public sealed record AtQrTaxBreakdown(
    IReadOnlyList<AtQrTaxBucket> Buckets,
    decimal NonSubjectAmount = 0m)
{
    public static AtQrTaxBreakdown Empty { get; } = new([], 0m);

    /// <summary>
    /// Groups line totals by <c>(CountryRegion, TaxCode)</c>.
    /// Unknown tax codes (not ISE/RED/INT/NOR) are summed into <see cref="NonSubjectAmount"/> (field L).
    /// </summary>
    public static AtQrTaxBreakdown FromLines(
        IEnumerable<(string CountryRegion, string TaxCode, decimal TotalNet, decimal TotalTax)> lines)
    {
        var buckets = new Dictionary<(string Region, string Code), (decimal Net, decimal Tax)>(
            RegionCodeComparer.Instance);
        var nonSubject = 0m;

        foreach (var line in lines)
        {
            var region = string.IsNullOrWhiteSpace(line.CountryRegion) ? "PT" : line.CountryRegion.Trim();
            var code = string.IsNullOrWhiteSpace(line.TaxCode)
                ? string.Empty
                : line.TaxCode.Trim().ToUpperInvariant();
            if (code is not ("ISE" or "RED" or "INT" or "NOR"))
            {
                nonSubject += Math.Abs(line.TotalNet);
                continue;
            }

            var key = (region, code);
            if (buckets.TryGetValue(key, out var existing))
            {
                buckets[key] = (existing.Net + line.TotalNet, existing.Tax + line.TotalTax);
            }
            else
            {
                buckets[key] = (line.TotalNet, line.TotalTax);
            }
        }

        var list = buckets
            .Select(pair => new AtQrTaxBucket(pair.Key.Region, pair.Key.Code, pair.Value.Net, pair.Value.Tax))
            .ToArray();
        return new AtQrTaxBreakdown(list, nonSubject);
    }

    /// <summary>
    /// Payment / receipt with no per-rate VAT discrimination (spec field L — “recibos gerais
    /// sem discriminação de imposto”). Yields empty I/J/K buckets so the builder emits <c>I1:0</c>
    /// and places <paramref name="amountWithoutRateSplit"/> in L.
    /// </summary>
    public static AtQrTaxBreakdown FromUndiscriminatedPayment(decimal amountWithoutRateSplit)
        => new([], Math.Abs(amountWithoutRateSplit));

    private sealed class RegionCodeComparer : IEqualityComparer<(string Region, string Code)>
    {
        public static RegionCodeComparer Instance { get; } = new();

        public bool Equals((string Region, string Code) x, (string Region, string Code) y)
            => string.Equals(x.Region, y.Region, StringComparison.OrdinalIgnoreCase)
               && string.Equals(x.Code, y.Code, StringComparison.Ordinal);

        public int GetHashCode((string Region, string Code) obj)
            => HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Region),
                StringComparer.Ordinal.GetHashCode(obj.Code));
    }
}

/// <summary>
/// Pure in-memory input for the AT QR payload (no database access).
/// Monetary fields are in document currency unless <see cref="CurrencyAcronym"/> is EUR/null
/// (then treated as already EUR). The builder multiplies by <see cref="ExchangeRate"/> when
/// the acronym is a foreign currency (same convention as <c>Document.ExchangeRate</c>).
/// </summary>
public sealed record AtQrPayloadInput(
    string? IssuerFiscalNumber,
    string? CustomerFiscalNumber,
    string? CustomerCountry,
    string DocumentType,
    string Status,
    string DateYyyyMmDd,
    string UniqueDocumentId,
    string? Atcud,
    AtQrTaxBreakdown TaxBreakdown,
    decimal TotalTax,
    decimal TotalFinal,
    string? Hash4,
    decimal? WithholdingTaxAmount = null,
    decimal StampDutyAmount = 0m,
    string? CurrencyAcronym = null,
    decimal ExchangeRate = 1m,
    string? OtherInfo = null,
    string? CertificateNumber = null);

/// <summary>
/// Builds the Portuguese AT QR Code payload (fields A–S) per Portaria 195/2020 v1.1.
/// Shared by the API <c>DocumentQrCodeGenerator</c>, the certified fiscal plugin, and Desktop.
/// </summary>
public static class DocumentAtQrPayload
{
    private static readonly string[] RegionOrder = ["PT", "PT-AC", "PT-MA"];
    private static readonly string[] SpaceLetters = ["I", "J", "K"];

    /// <summary>Preferred entry point: pure <see cref="AtQrPayloadInput"/> (no DB).</summary>
    public static string Build(AtQrPayloadInput input)
    {
        ArgumentNullException.ThrowIfNull(input);
        var breakdown = input.TaxBreakdown ?? AtQrTaxBreakdown.Empty;
        var toEur = CreateEurConverter(input.CurrencyAcronym, input.ExchangeRate);

        var fields = new List<string>
        {
            "A:" + input.IssuerFiscalNumber,
            "B:" + input.CustomerFiscalNumber,
            "C:" + input.CustomerCountry,
            "D:" + input.DocumentType,
            "E:" + input.Status,
            "F:" + input.DateYyyyMmDd.Replace("-", "", StringComparison.Ordinal),
            "G:" + input.UniqueDocumentId,
            "H:" + (string.IsNullOrWhiteSpace(input.Atcud) ? "0" : input.Atcud)
        };

        AppendFiscalSpaces(fields, breakdown.Buckets, toEur);

        var nonSubject = toEur(breakdown.NonSubjectAmount);
        if (nonSubject != 0m)
        {
            fields.Add("L:" + FormatAmount(nonSubject));
        }

        var stampDuty = toEur(input.StampDutyAmount);
        if (stampDuty != 0m)
        {
            fields.Add("M:" + FormatAmount(stampDuty));
        }

        fields.Add("N:" + FormatAmount(toEur(input.TotalTax)));
        fields.Add("O:" + FormatAmount(toEur(input.TotalFinal)));

        var withholding = input.WithholdingTaxAmount is decimal amount ? toEur(amount).Round() : (decimal?)null;
        if (withholding is decimal retained && retained != 0m)
        {
            fields.Add("P:" + FormatAmount(retained));
        }

        fields.Add("Q:" + input.Hash4);
        fields.Add("R:" + (input.CertificateNumber ?? SoftwareInfo.Portugal.SaftCertificateNumber));

        if (string.IsNullOrWhiteSpace(input.OtherInfo) == false)
        {
            fields.Add("S:" + input.OtherInfo);
        }

        return string.Join("*", fields);
    }

    /// <summary>
    /// Obsolete compatibility overload for callers that only have document totals.
    /// Prefer <see cref="Build(AtQrPayloadInput)"/>. Maps tax&gt;0 → PT/NOR, tax==0 and net&gt;0 → PT/ISE,
    /// both 0 → empty breakdown (<c>I1:0</c>). Does not fix INT/RED/PT-AC without line totals.
    /// </summary>
    [Obsolete("Use DocumentAtQrPayload.Build(AtQrPayloadInput). For RC/RG prefer AtQrReceiptBreakdown.FromSettledDocuments (or AtQrTaxBreakdown.FromLines / FromUndiscriminatedPayment).", error: false)]
    public static string Build(
        string? issuerFiscalNumber,
        string? customerFiscalNumber,
        string? customerCountry,
        string documentType,
        string status,
        string dateYyyyMmDd,
        string uniqueDocumentId,
        string? atcud,
        decimal totalNet,
        decimal totalTax,
        decimal totalFinal,
        string? hash4)
    {
        AtQrTaxBreakdown breakdown;
        if (totalNet == 0m && totalTax == 0m)
        {
            breakdown = AtQrTaxBreakdown.Empty;
        }
        else if (totalTax == 0m)
        {
            breakdown = new AtQrTaxBreakdown([new AtQrTaxBucket("PT", "ISE", totalNet, 0m)]);
        }
        else
        {
            breakdown = new AtQrTaxBreakdown([new AtQrTaxBucket("PT", "NOR", totalNet, totalTax)]);
        }

        return Build(new AtQrPayloadInput(
            issuerFiscalNumber,
            customerFiscalNumber,
            customerCountry,
            documentType,
            status,
            dateYyyyMmDd,
            uniqueDocumentId,
            atcud,
            breakdown,
            totalTax,
            totalFinal,
            hash4));
    }

    /// <summary>
    /// Converts a document-currency amount to EUR using the document exchange rate.
    /// <paramref name="currencyAcronym"/> null/empty/EUR → no conversion; otherwise amount × rate.
    /// </summary>
    public static decimal ToEur(decimal amount, string? currencyAcronym, decimal exchangeRate)
        => CreateEurConverter(currencyAcronym, exchangeRate)(amount);

    /// <summary>Obsolete alias — use <see cref="AtQrTaxBreakdown.FromLines"/>.</summary>
    [Obsolete("Use AtQrTaxBreakdown.FromLines.")]
    public static AtQrTaxBucket[] AggregateTaxTotals(
        IEnumerable<(string CountryRegion, string TaxCode, decimal TotalNet, decimal TotalTax)> lines,
        out decimal nonSubjectAmount)
    {
        var breakdown = AtQrTaxBreakdown.FromLines(lines);
        nonSubjectAmount = breakdown.NonSubjectAmount;
        return breakdown.Buckets.ToArray();
    }

    private static void AppendFiscalSpaces(
        List<string> fields,
        IReadOnlyList<AtQrTaxBucket> taxTotals,
        Func<decimal, decimal> toEur)
    {
        var byRegion = new Dictionary<string, Dictionary<string, (decimal Base, decimal Tax)>>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var total in taxTotals)
        {
            var region = string.IsNullOrWhiteSpace(total.CountryRegion) ? "PT" : total.CountryRegion.Trim();
            var code = string.IsNullOrWhiteSpace(total.TaxCode)
                ? string.Empty
                : total.TaxCode.Trim().ToUpperInvariant();
            if (code is not ("ISE" or "RED" or "INT" or "NOR"))
            {
                continue;
            }

            if (byRegion.TryGetValue(region, out var codes) == false)
            {
                codes = new Dictionary<string, (decimal Base, decimal Tax)>(StringComparer.Ordinal);
                byRegion[region] = codes;
            }

            var taxable = toEur(total.TaxableBase);
            var tax = toEur(total.TaxAmount);
            if (codes.TryGetValue(code, out var existing))
            {
                codes[code] = (existing.Base + taxable, existing.Tax + tax);
            }
            else
            {
                codes[code] = (taxable, tax);
            }
        }

        if (byRegion.Count == 0)
        {
            fields.Add("I1:0");
            return;
        }

        var letterByRegion = AssignSpaceLetters(byRegion.Keys);
        foreach (var region in OrderRegions(byRegion.Keys))
        {
            var letter = letterByRegion[region];
            var codes = byRegion[region];
            fields.Add(letter + "1:" + region);

            if (codes.TryGetValue("ISE", out var ise) && ise.Base != 0m)
            {
                fields.Add(letter + "2:" + FormatAmount(ise.Base));
            }

            if (codes.TryGetValue("RED", out var red))
            {
                if (red.Base != 0m)
                {
                    fields.Add(letter + "3:" + FormatAmount(red.Base));
                }

                if (red.Tax != 0m)
                {
                    fields.Add(letter + "4:" + FormatAmount(red.Tax));
                }
            }

            if (codes.TryGetValue("INT", out var intermediate))
            {
                if (intermediate.Base != 0m)
                {
                    fields.Add(letter + "5:" + FormatAmount(intermediate.Base));
                }

                if (intermediate.Tax != 0m)
                {
                    fields.Add(letter + "6:" + FormatAmount(intermediate.Tax));
                }
            }

            if (codes.TryGetValue("NOR", out var normal))
            {
                if (normal.Base != 0m)
                {
                    fields.Add(letter + "7:" + FormatAmount(normal.Base));
                }

                if (normal.Tax != 0m)
                {
                    fields.Add(letter + "8:" + FormatAmount(normal.Tax));
                }
            }
        }
    }

    private static Dictionary<string, string> AssignSpaceLetters(IEnumerable<string> regions)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var used = new HashSet<string>(StringComparer.Ordinal);

        foreach (var known in RegionOrder)
        {
            if (regions.Any(r => string.Equals(r, known, StringComparison.OrdinalIgnoreCase)) == false)
            {
                continue;
            }

            var letter = known switch
            {
                "PT" => "I",
                "PT-AC" => "J",
                "PT-MA" => "K",
                _ => "I"
            };
            map[known] = letter;
            used.Add(letter);
        }

        foreach (var region in OrderRegions(regions))
        {
            if (map.ContainsKey(region))
            {
                continue;
            }

            var letter = SpaceLetters.FirstOrDefault(candidate => used.Contains(candidate) == false) ?? "I";
            map[region] = letter;
            used.Add(letter);
        }

        return map;
    }

    private static IEnumerable<string> OrderRegions(IEnumerable<string> regions)
    {
        return regions
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(region =>
            {
                var index = Array.FindIndex(
                    RegionOrder,
                    known => string.Equals(known, region, StringComparison.OrdinalIgnoreCase));
                return index < 0 ? 100 : index;
            }, Comparer<int>.Default)
            .ThenBy(region => region, StringComparer.OrdinalIgnoreCase);
    }

    private static Func<decimal, decimal> CreateEurConverter(string? currencyAcronym, decimal exchangeRate)
    {
        var isEur = string.IsNullOrWhiteSpace(currencyAcronym)
                    || currencyAcronym.Equals("EUR", StringComparison.OrdinalIgnoreCase);
        var rate = exchangeRate > 0m ? exchangeRate : 1m;
        return amount =>
        {
            var absolute = Math.Abs(amount);
            return isEur ? absolute : absolute * rate;
        };
    }

    private static string FormatAmount(decimal amount)
        => amount.To2DecimalPlacesString();
}
