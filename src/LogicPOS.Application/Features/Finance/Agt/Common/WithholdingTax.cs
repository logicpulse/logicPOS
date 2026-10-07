

using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.Common;

public record WithholdingTax
{
    [JsonProperty("withholdingTaxType")] public string? WithholdingTaxType { get; set; } 
    
    [JsonProperty("withholdingTaxDescription")] public string? WithholdingTaxDescription { get; set; } 
    
    [JsonProperty("withholdingTaxAmount")] public string? WithholdingTaxAmount { get; set; }
}