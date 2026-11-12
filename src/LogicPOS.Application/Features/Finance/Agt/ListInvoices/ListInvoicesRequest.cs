
using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.ListInvoices;

public record ListInvoicesRequest : AgtCommunicationServiceRequest
{
    [JsonProperty("jwsSignature")] public string JwsSignature { get; set; } = null!;
    [JsonProperty("queryStartDate")] public string QueryStartDate { get; set; } = null!;
    [JsonProperty("queryEndDate")] public string QueryEndDate { get; set; } = null!;

    /// <summary>EMITIDAS or RECEBIDAS. Optional for AGT; defaults to EMITIDAS when listing from LogicPOS.</summary>
    [JsonProperty("tipoListagem")] public string? ListingType { get; set; }

    [JsonProperty("page")] public int? Page { get; set; }

    [JsonProperty("size")] public int? Size { get; set; }
}