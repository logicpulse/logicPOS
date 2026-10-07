using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.ListSeries;

public record ListSeriesResponse
{
    [JsonProperty("ResultCode")] public string? ResultCode { get; set; } 
    [JsonProperty("errorList")] public List<Error>? ErrorList { get; set; } 

    [JsonProperty("seriesResultCount")] public string? SeriesResultCount { get; set; }
    [JsonProperty("seriesInfo")] public List<SeriesInfo>? SeriesInfo { get; set; }    
}

public record SeriesInfo
{
    [JsonProperty("id")] public string? Id { get; set; } 
    [JsonProperty("seriesCode")] public string? SeriesCode { get; set; } 
    [JsonProperty("seriesYear")] public string? SeriesYear { get; set; } 
    [JsonProperty("documentType")] public string? DocumentType { get; set; }
    [JsonProperty("seriesStatus")] public string? SeriesStatus { get; set; }
    [JsonProperty("seriesCreationDate")] public string? SeriesCreationDate { get; set; }
    [JsonProperty("firstDocumentCreated")] public string? FirstDocumentCreated { get; set; }
    [JsonProperty("lastDocumentCreated")] public string? LastDocumentCreated { get; set; }
    [JsonProperty("invoicingMethod")] public string? InvoicingMethod { get; set; }
    [JsonProperty("seriesContingencyIndicator")] public string? SeriesContingencyIndicator { get; set; }
    [JsonProperty("nif")] public string? Nif { get; set; }
    [JsonProperty("nome")] public string? Name { get; set; }
    [JsonProperty("dataAdesao")] public string? JoiningDate { get; set; }
    [JsonProperty("tipoAdesao")] public string? JoiningType { get; set; }
}

public record Error
{
    [JsonProperty("idError")] public string IdError { get; set; } = null!;
    [JsonProperty("descriptionError")] public string DescriptionError { get; set; } = null!;
}