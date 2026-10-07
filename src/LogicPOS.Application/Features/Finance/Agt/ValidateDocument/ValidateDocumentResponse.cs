
using LogicPOS.Application.Features.Finance.Agt.RegisterInvoice;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.ValidateDocument;

public record ValidateDocumentResponse
{
    [JsonProperty("actionResultCode")] public string? ActionResultCode { get; set; } 
    [JsonProperty("documentStatusCode")] public string? DocumentStatusCode { get; set; }
    [JsonProperty("errorList")] public List<ErrorListItem>? ErrorList { get; set; } 
    
    public string GetCompactErrorMessage()
    {
        return string.Join(";\n\n", ErrorList?.Select(x => $"{x.IdError}: {x.DescriptionError}") ?? []);
    }
}

public record ErrorListItem
{
    [JsonProperty("idError")] public string IdError { get; init; } = null!;
    [JsonProperty("descriptionError")] public string DescriptionError { get; init; } = null!;
    [JsonProperty("documentNo")] public string? DocumentNo { get; init; }
}