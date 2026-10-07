

using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.Common;

public record DocumentLineTax
{
    [JsonProperty("taxType")] public string? TaxType { get; set; }

    [JsonProperty("taxCountryRegion")] public string? TaxCountryRegion { get; set; }

    [JsonProperty("taxCode")] public string? TaxCode { get; set; } 

    [JsonProperty("taxBase")] public string? TaxBase{ get; set; } 
    
    [JsonProperty("taxPercentage")] public string? TaxPercentage { get; set; } 
    
    [JsonProperty("taxAmount")] public string? TaxAmount { get; set; } 

    [JsonProperty("taxContribution")] public string? TaxContribution { get; set; } 
    
    [JsonProperty("taxExemptionCode")] public string? TaxExemptionCode { get; set; } 
}