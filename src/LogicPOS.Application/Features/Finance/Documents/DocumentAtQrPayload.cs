using LogicPOS.Domain.Entities.Finance.Calculations;
using LogicPOS.Shared.Features.Software;

namespace LogicPOS.Application.Features.Finance.Documents;

/// <summary>
/// One VAT / tax bucket for the AT QR payload (fields I/J/K).
/// <paramref name="TaxCode"/> is the SAF-T code: ISE, RED, INT, NOR (or other → L).
/// </summary>
public readonly record struct DocumentAtQrTaxTotal(
    string CountryRegion,
    string TaxCode,
    decimal TaxableBase,
    decimal TaxAmount);

/// <summary>
/// Builds the Portuguese AT QR Code payload (fields A–S) per Portaria 195/2020 v1.1.
/// Shared by the API <c>DocumentQrCodeGenerator</c> and the certified fiscal plugin.
/// </summary>
public static class DocumentAtQrPayload
{
    private static readonly string[] RegionOrder = ["PT", "PT-AC", "PT-MA"];
    private static readonly string[] SpaceLetters = ["I", "J", "K"];

    /// <summary>
    /// Compatibility overload used by callers that only have document totals.
    /// Prefer <see cref="Build(string?,string?,string?,string,string,string,string,string?,System.Collections.Generic.IEnumerable{DocumentAtQrTaxTotal},decimal,decimal,string?,decimal,decimal,decimal,string?,decimal,string?,string?)"/>
    /// with per-line (region, tax code) totals. Without a breakdown, a positive tax amount is
    /// mapped to PT/NOR and a zero-tax positive base to PT/ISE (INT/RED and non-PT regions require the new overload).
    /// </summary>
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
        DocumentAtQrTaxTotal[] taxTotals;
        if (totalNet == 0m && totalTax == 0m)
        {
            taxTotals = [];
        }
        else if (totalTax == 0m)
        {
            taxTotals = [new DocumentAtQrTaxTotal("PT", "ISE", totalNet, 0m)];
        }
        else
        {
            taxTotals = [new DocumentAtQrTaxTotal("PT", "NOR", totalNet, totalTax)];
        }

        return Build(
            issuerFiscalNumber,
            customerFiscalNumber,
            customerCountry,
            documentType,
            status,
            dateYyyyMmDd,
            uniqueDocumentId,
            atcud,
            taxTotals,
            totalTax,
            totalFinal,
            hash4);
    }

    /// <summary>
    /// Builds the AT QR payload string per Portaria 195/2020 v1.1.
    /// Callers supply the four hash characters (positions 1/11/21/31) and tax totals
    /// grouped by <c>(CountryRegion, TaxCode)</c> from the document lines.
    /// </summary>
    public static string Build(
        string? issuerFiscalNumber,
        string? customerFiscalNumber,
        string? customerCountry,
        string documentType,
        string status,
        string dateYyyyMmDd,
        string uniqueDocumentId,
        string? atcud,
        IEnumerable<DocumentAtQrTaxTotal> taxTotals,
        decimal totalTax,
        decimal totalFinal,
        string? hash4,
        decimal withholdingTaxAmount = 0m,
        decimal nonSubjectAmount = 0m,
        decimal stampDutyAmount = 0m,
        string? currencyAcronym = null,
        decimal exchangeRate = 1m,
        string? otherInfo = null,
        string? certificateNumber = null)
    {
        var toEur = CreateEurConverter(currencyAcronym, exchangeRate);
        var fields = new List<string>
        {
            "A:" + issuerFiscalNumber,
            "B:" + customerFiscalNumber,
            "C:" + customerCountry,
            "D:" + documentType,
            "E:" + status,
            "F:" + dateYyyyMmDd.Replace("-", "", StringComparison.Ordinal),
            "G:" + uniqueDocumentId,
            "H:" + (string.IsNullOrWhiteSpace(atcud) ? "0" : atcud)
        };

        AppendFiscalSpaces(fields, taxTotals, toEur);

        var nonSubject = toEur(nonSubjectAmount);
        if (nonSubject != 0m)
        {
            fields.Add("L:" + FormatAmount(nonSubject));
        }

        var stampDuty = toEur(stampDutyAmount);
        if (stampDuty != 0m)
        {
            fields.Add("M:" + FormatAmount(stampDuty));
        }

        fields.Add("N:" + FormatAmount(toEur(totalTax)));
        fields.Add("O:" + FormatAmount(toEur(totalFinal)));

        var withholding = toEur(withholdingTaxAmount);
        if (withholding != 0m)
        {
            fields.Add("P:" + FormatAmount(withholding));
        }

        fields.Add("Q:" + hash4);
        fields.Add("R:" + (certificateNumber ?? SoftwareInfo.Portugal.SaftCertificateNumber));

        if (string.IsNullOrWhiteSpace(otherInfo) == false)
        {
            fields.Add("S:" + otherInfo);
        }

        return string.Join("*", fields);
    }

    /// <summary>
    /// Aggregates document-line tax amounts by <c>(CountryRegion, TaxCode)</c>.
    /// Unknown tax codes (not ISE/RED/INT/NOR) are summed into <paramref name="nonSubjectAmount"/> (field L).
    /// </summary>
    public static DocumentAtQrTaxTotal[] AggregateTaxTotals(
        IEnumerable<(string CountryRegion, string TaxCode, decimal TotalNet, decimal TotalTax)> lines,
        out decimal nonSubjectAmount)
    {
        nonSubjectAmount = 0m;
        var buckets = new Dictionary<(string Region, string Code), (decimal Net, decimal Tax)>(
            RegionCodeComparer.Instance);

        foreach (var line in lines)
        {
            var region = string.IsNullOrWhiteSpace(line.CountryRegion) ? "PT" : line.CountryRegion.Trim();
            var code = string.IsNullOrWhiteSpace(line.TaxCode) ? string.Empty : line.TaxCode.Trim().ToUpperInvariant();
            if (code is not ("ISE" or "RED" or "INT" or "NOR"))
            {
                nonSubjectAmount += Math.Abs(line.TotalNet);
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

        return buckets
            .Select(pair => new DocumentAtQrTaxTotal(pair.Key.Region, pair.Key.Code, pair.Value.Net, pair.Value.Tax))
            .ToArray();
    }

    private static void AppendFiscalSpaces(
        List<string> fields,
        IEnumerable<DocumentAtQrTaxTotal> taxTotals,
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
