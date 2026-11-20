namespace LogicPOS.Core.Finance.Postal;

/// <summary>HTTP access to <c>json.geoapi.pt</c> for tests with canned JSON.</summary>
public interface IPortugalGeoApiClient
{
    Task<string?> GetPostalCodeJsonAsync(string cp, CancellationToken cancellationToken = default);
}