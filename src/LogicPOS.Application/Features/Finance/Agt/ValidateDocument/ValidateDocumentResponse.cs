using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.ValidateDocument;

public record ValidateDocumentResponse
{
    [JsonProperty("confirmRejectResult")]
    public ConfirmRejectResult? ConfirmRejectResult { get; set; }

    [JsonProperty("errorList")]
    public List<ErrorListItem>? ErrorList { get; set; }

    [JsonIgnore]
    public string? ActionResultCode => ConfirmRejectResult?.ActionResultCode;

    [JsonIgnore]
    public string? DocumentStatusCode => ConfirmRejectResult?.DocumentStatusCode;

    public string GetCompactErrorMessage()
    {
        return string.Join(";\n\n", ErrorList?.Select(x => $"{x.IdError}: {x.DescriptionError}") ?? []);
    }
}

public record ConfirmRejectResult
{
    [JsonProperty("actionResultCode")]
    public string? ActionResultCode { get; set; }

    [JsonProperty("documentStatusCode")]
    public string? DocumentStatusCode { get; set; }
}

public record ErrorListItem
{
    [JsonProperty("idError")] public string IdError { get; init; } = null!;
    [JsonProperty("descriptionError")] public string DescriptionError { get; init; } = null!;
    [JsonProperty("documentNo")] public string? DocumentNo { get; init; }
}
