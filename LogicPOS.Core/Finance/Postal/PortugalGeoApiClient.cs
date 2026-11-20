using System.Net;

namespace LogicPOS.Core.Finance.Postal;

public sealed class PortugalGeoApiClient : IPortugalGeoApiClient
{
    private readonly HttpClient _http;

    public PortugalGeoApiClient()
        : this(CreateClient())
    {
    }

    public PortugalGeoApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<string?> GetPostalCodeJsonAsync(string cp, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cp))
        {
            return null;
        }

        using var response = await _http.GetAsync("cp/" + Uri.EscapeDataString(cp.Trim()), cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }

    private static HttpClient CreateClient()
        => new()
        {
            BaseAddress = new Uri("https://json.geoapi.pt/"),
            Timeout = TimeSpan.FromSeconds(15)
        };
}