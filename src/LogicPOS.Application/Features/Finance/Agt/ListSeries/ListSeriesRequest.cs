
using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.ListSeries;

public record ListSeriesRequest : AgtCommunicationServiceRequest
{
    [JsonProperty("seriesCode")] public string? SeriesCode { get; set; }
    [JsonProperty("seriesYear")] public string? SeriesYear { get; set; } 
    [JsonProperty("seriesStatus")] public string? SeriesStatus { get; set; }
    [JsonProperty("documentType")] public string? DocumentType { get; set; } 
    [JsonProperty("establishmentNumber")] public string? EstablishmentNumber { get; set; }
    [JsonProperty("jwsSignature")] public string JwsSignature { get; set; } = null!;
}