
using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.RequestSeries;

public record RequestSeriesRequest : AgtCommunicationServiceRequest
{
    [JsonProperty("jwsSignature")] public string JwsSignature { get; set; } = null!;
    [JsonProperty("seriesYear")] public string SeriesYear { get; set; } = null!;
    [JsonProperty("documentType")] public string DocumentType { get; set; } = null!;
    [JsonProperty("establishmentNumber")] public string EstablishmentNumber { get; set; } = null!;
    [JsonProperty("seriesContingencyIndicator")] public string SeriesContingencyIndicator { get; set; } = null!;
}