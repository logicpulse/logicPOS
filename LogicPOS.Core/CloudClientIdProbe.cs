using System.Net;
using System.Net.Http;

namespace LogicPOS.Core;

/// <summary>
/// Confirms a Cloud Client ID maps to an active LogicPOS Cloud account on the production API.
/// </summary>
public static class CloudClientIdProbe
{
    public const string ProductionApiBase = "https://api.logic-pos.com/";

    public static async Task<(bool Ok, string? Error)> ValidateAsync(
        string clientId,
        string? apiBaseAddress = null,
        CancellationToken cancellationToken = default)
    {
        var trimmed = clientId.Trim();
        if (string.IsNullOrWhiteSpace(trimmed))
            return (false, "Indique o Client ID da conta Cloud.");

        var baseUrl = string.IsNullOrWhiteSpace(apiBaseAddress)
            ? ProductionApiBase
            : apiBaseAddress.Trim().TrimEnd('/') + "/";

        try
        {
            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                new Uri(new Uri(baseUrl), "system/api-version"));
            request.Headers.TryAddWithoutValidation("X-Client-Id", trimmed);

            using var response = await http.SendAsync(request, cancellationToken).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
                return (true, null);

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return (false,
                    "A conta Cloud não existe ou não está activa. Crie / pague em logic-pos.com e volte a tentar.");
            }

            return (false, $"Não foi possível validar a conta Cloud ({(int)response.StatusCode}).");
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception exception)
        {
            return (false, $"Sem ligação à API Cloud: {exception.Message}");
        }
    }
}
