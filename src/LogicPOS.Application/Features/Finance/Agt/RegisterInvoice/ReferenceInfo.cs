

using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.RegisterInvoice;

public record ReferenceInfo
{
    [JsonProperty("reference")] public string Reference { get; set; } = null!;

    [JsonProperty("referenceItemLineNo")]
    public string? ReferenceItemLineNo { get; set; }

    [JsonProperty("reason")] public string? Reason { get; set; }
}