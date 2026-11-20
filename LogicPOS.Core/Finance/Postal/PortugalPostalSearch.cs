using LogicPOS.Core.BackOffice;

namespace LogicPOS.Core.Finance.Postal;

public sealed class PortugalPostalSearch
{
    private readonly IPortugalGeoApiClient _client;

    public PortugalPostalSearch(IPortugalGeoApiClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<PostalLocalityMatch>> SearchAsync(
        string search,
        PostalSearchTrigger trigger,
        int take = 25,
        CancellationToken cancellationToken = default)
    {
        var term = (search ?? string.Empty).Trim();
        if (term.Length == 0)
        {
            return [];
        }

        var complete = PortuguesePostalFormats.TryNormalizeComplete(term, out var normalized);
        var prefix = PortuguesePostalFormats.IsPrefixOnly(term);

        if (trigger == PostalSearchTrigger.AutoComplete)
        {
            if (complete == false)
            {
                return [];
            }

            return await FetchAsync(normalized, take, cancellationToken);
        }

        if (complete)
        {
            return await FetchAsync(normalized, take, cancellationToken);
        }

        if (prefix)
        {
            return await FetchAsync(term, take, cancellationToken);
        }

        return [];
    }

    private async Task<IReadOnlyList<PostalLocalityMatch>> FetchAsync(
        string cp,
        int take,
        CancellationToken cancellationToken)
    {
        string? json;
        try
        {
            json = await _client.GetPostalCodeJsonAsync(cp, cancellationToken);
        }
        catch (HttpRequestException)
        {
            return [];
        }
        catch (TaskCanceledException)
        {
            return [];
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        return PortugalGeoApiPostalParser.Parse(json, take);
    }
}