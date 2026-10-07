
using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.ListInvoices;

public record ListInvoicesRequest : AgtCommunicationServiceRequest
{
    [JsonProperty("jwsSignature")] public string JwsSignature { get; set; } = null!;
    [JsonProperty("queryStartDate")] public string QueryStartDate { get; set; } = null!;
    [JsonProperty("queryEndDate")] public string QueryEndDate { get; set; } = null!;
}