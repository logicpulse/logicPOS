using System.Text.Json;

namespace LogicPOS.Api.Features.Common.Requests
{
    internal static class ApiJsonSerializerOptions
    {
        public static readonly JsonSerializerOptions Default = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }
}
