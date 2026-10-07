

using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.RegisterInvoice;

public record SourceDocumentId
{
    [JsonProperty("originatingON")] public string OriginatingOn { get; set; } = null!;
    [JsonProperty("documentDate")] public string DocumentDate { get; set; } = null!;
}