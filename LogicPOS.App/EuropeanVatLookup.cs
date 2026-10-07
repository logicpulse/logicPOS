using System.Globalization;
using System.Xml.Linq;

namespace LogicPOS.App;

/// <summary>
/// Reads a European VAT number from the public VIES service, the same source the GTK back office uses.
/// </summary>
internal static class EuropeanVatLookup
{
    private const string Endpoint = "https://ec.europa.eu/taxation_customs/vies/services/checkVatService";

    public static async Task<EuropeanVat?> FindAsync(string countryCode, string vatNumber, CancellationToken cancellationToken = default)
    {
        countryCode = countryCode.Trim().ToUpperInvariant();
        vatNumber = new string(vatNumber.Where(char.IsLetterOrDigit).ToArray());
        if (countryCode.Length != 2 || vatNumber.Length < 2)
        {
            return null;
        }

        var body = $"""
            <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:urn="urn:ec.europa.eu:taxud:vies:services:checkVat:types">
              <soapenv:Header/>
              <soapenv:Body>
                <urn:checkVat>
                  <urn:countryCode>{countryCode}</urn:countryCode>
                  <urn:vatNumber>{vatNumber}</urn:vatNumber>
                </urn:checkVat>
              </soapenv:Body>
            </soapenv:Envelope>
            """;

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
            using var content = new StringContent(body, System.Text.Encoding.UTF8, "text/xml");
            using var response = await client.PostAsync(Endpoint, content, cancellationToken);
            if (response.IsSuccessStatusCode == false)
            {
                return null;
            }

            var document = XDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            var payload = document.Descendants().FirstOrDefault(element => element.Name.LocalName == "checkVatResponse");
            if (payload is null)
            {
                return null;
            }

            var valid = Text(payload, "valid");
            if (string.Equals(valid, "true", StringComparison.OrdinalIgnoreCase) == false)
            {
                return null;
            }

            var name = Title(Clean(Text(payload, "name")));
            if (string.IsNullOrWhiteSpace(name) || name == "---")
            {
                return null;
            }

            var lines = Clean(Text(payload, "address"))
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            var postal = lines.Length > 2 ? lines[2] : string.Empty;
            var space = postal.IndexOf(' ');
            return new EuropeanVat
            {
                Name = name,
                Address = lines.Length > 0 ? Title(lines[0]) : string.Empty,
                City = lines.Length > 1 ? Title(lines[1]) : string.Empty,
                ZipCode = space > 0 ? postal[..space] : postal,
                Locality = space > 0 ? Title(postal[(space + 1)..]) : string.Empty
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static string Text(XElement parent, string name)
        => parent.Elements().FirstOrDefault(element => element.Name.LocalName == name)?.Value ?? string.Empty;

    private static string Clean(string value)
        => value.Replace("Ã‡", "ç", StringComparison.Ordinal).Replace('?', 'ç').Replace('Æ', 'ã').Replace('¢', 'ó').Replace('¡', 'í').Trim();

    private static string Title(string value)
        => string.IsNullOrWhiteSpace(value) ? string.Empty : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower(CultureInfo.CurrentCulture));
}

internal sealed class EuropeanVat
{
    public string Name { get; init; } = string.Empty;

    public string Address { get; init; } = string.Empty;

    public string City { get; init; } = string.Empty;

    public string ZipCode { get; init; } = string.Empty;

    public string Locality { get; init; } = string.Empty;
}
