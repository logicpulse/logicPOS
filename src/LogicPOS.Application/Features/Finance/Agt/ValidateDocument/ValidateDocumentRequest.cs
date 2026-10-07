
using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.ValidateDocument;

public record ValidateDocumentRequest : AgtCommunicationServiceRequest
{
    [JsonProperty("jwsSignature")] public string JwsSignature { get; set; } = null!;
    [JsonProperty("documentNo")] public string DocumentNo { get; set; } = null!;
    [JsonProperty("action")] public string Action { get; set; } = null!;
    [JsonProperty("deductibleVATPercentage")] public decimal? DeductibleVatPercentage { get; set; } 
    [JsonProperty("nonDeductibleAmount")] public decimal? NonDeductibleAmount { get; set; } 
}