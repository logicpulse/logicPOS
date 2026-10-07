
using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.RegisterInvoice;

public record RegisterInvoiceRequest : AgtCommunicationServiceRequest
{

    [JsonProperty("numberOfEntries")] public string NumberOfEntries { get; set; } = null!;

    [JsonProperty("documents")] public List<Document> Documents { get; set; } = null!;
}