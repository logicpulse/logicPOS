using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.GetInvoice;

public record GetInvoiceRequest : AgtCommunicationServiceRequest
{
    [JsonProperty("jwsSignature")] public string JwsSignature { get; set; } = null!;
    [JsonProperty("invoiceNo")] public string InvoiceNo { get; set; } = null!;
}