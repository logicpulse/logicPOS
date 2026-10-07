

using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.Common;

public record DocumentTotals
{
    [JsonProperty("taxPayable")] public string TaxPayable { get; set; } = null!;
    [JsonProperty("netTotal")] public string NetTotal { get; set; } = null!;
    [JsonProperty("currency")] public Currency? Currency { get; set; } 
    [JsonProperty("grossTotal")] public string GrossTotal { get; set; } = null!;
}