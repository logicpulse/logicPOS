
using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.GetInvoiceStatus;

public record GetInvoiceStatusRequest : AgtCommunicationServiceRequest
{
    [JsonProperty("requestID")] public string RequestId { get; set; } = null!;
    [JsonProperty("jwsSignature")] public string JwsSignature { get; set; } = null!;
}