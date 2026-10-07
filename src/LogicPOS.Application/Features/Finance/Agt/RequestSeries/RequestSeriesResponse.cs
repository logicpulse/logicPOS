using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.RequestSeries;

public record RequestSeriesResponse
{
    [JsonProperty("resultCode")] public int? ResultCode { get; set; }
    [JsonProperty("errorList")] public List<Error>? ErrorList { get; set; }
    
    [JsonProperty("seriesFEResult")]
    public SeriesFeResult? SeriesFeResult { get; set; }
}

public record Error
{
    [JsonProperty("idError")] public string IdError { get; set; } = null!;
    [JsonProperty("descriptionError")] public string DescriptionError { get; set; } = null!;
}

public record SeriesFeResult
{
    [JsonProperty("seriesCode")] public string? SeriesCode { get; set; }

    [JsonProperty("authorizedQuantity")]
    public string? AuthorizedQuantity { get; set; }

    [JsonProperty("firstDocumentNo")] public string? FirstDocumentNo { get; set; }
    [JsonProperty("lastDocumentNo")] public string? LastDocumentNo { get; set; }
}