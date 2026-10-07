using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.GetInvoiceStatus;

public record GetInvoiceStatusResponse
{
    [JsonProperty("requestID")] public string? RequestId { get; set; } 

    [JsonProperty("taxRegistrationNumber")]
    public string? TaxRegistrationNumber { get; set; } 

    [JsonProperty("resultCode")] public string? ResultCode { get; set; }

    [JsonProperty("documentStatusList")]
    public List<DocumentStatusListItem>? DocumentStatusList { get; set; } 
    
    [JsonProperty("requestErrorList")] public List<Error>? RequestErrorList { get; set; } 
   
    [JsonProperty("successRequestID")] public string? SuccessRequestId { get; set; }

    public bool IsValid => ResultCode == "0";
    public bool HasSuccessRequestId => !string.IsNullOrEmpty(SuccessRequestId);
}

public record Error
{
    [JsonProperty("idError")] public string IdError { get; set; } = null!;
    [JsonProperty("descriptionError")] public string DescriptionError { get; set; } = null!;
}

public record DocumentStatusListItem
{
    [JsonProperty("documentNo")] public string DocumentNo { get; set; } = null!;
    [JsonProperty("documentStatus")] public string DocumentStatus { get; set; } = null!;
    [JsonProperty("errorList")] public List<Error>? ErrorList { get; set; } 
}


