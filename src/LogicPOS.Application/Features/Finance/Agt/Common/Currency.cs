

using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.Common;

public record Currency
{
    [JsonProperty("currencyAmount")] public string CurrencyAmount { get; set; } = null!;
    [JsonProperty("currencyCode")] public string CurrencyCode { get; set; } = null!;
    [JsonProperty("exchangeRate")] public string ExchangeRate { get; set; } = null!;
}