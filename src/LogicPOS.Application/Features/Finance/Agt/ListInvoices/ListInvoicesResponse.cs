using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.ListInvoices;

public record ListInvoicesResponse
{
    [JsonProperty("statusResult")] public StatusResult? StatusResult { get; set; } 
}

public record StatusResult
{
    [JsonProperty("documentResultCount")]
    public int DocumentResultCount { get; set; } 

    [JsonProperty("resultEntryList")]
    public List<ResultEntryListItem>? ResultEntryList { get; set; } 
    
}
public record ResultEntryListItem
{
     [JsonProperty("documentEntryResult")] public EntryResult? DocumentEntryResult { get; set; }
    
    public record EntryResult
    {
        [JsonProperty("id")] public string? Id { get; set; } 
        [JsonProperty("documentType")] public string? DocumentType { get; set; }
        [JsonProperty("documentTypeDescription")] public string? DocumentTypeDescription { get; set; }
        [JsonProperty("documentNo")] public string DocumentNo { get; set; } = null!;
        [JsonProperty("documentDate")] public string DocumentDate { get; set; } = null!;
        [JsonProperty("documentStatus")] public string? DocumentStatus { get; set; }

        [JsonProperty("documentStatusDescription")]
        public string? DocumentStatusDescription { get; set; }

        /// <summary>V confirmed, N pending, R rejected (AGT customerAcceptance / documentConfirmStatus).</summary>
        [JsonProperty("documentConfirmStatus")] public string? DocumentConfirmStatus { get; set; }

        [JsonProperty("netTotal")] public string? NetTotal { get; set; }
        [JsonProperty("grossTotal")] public string? GrossTotal { get; set; }
        [JsonProperty("taxPayable")] public string? TaxPayable { get; set; }
        [JsonProperty("customerTaxId")] public string? CustomerTaxId { get; set; }
        [JsonProperty("taxRegistrationNumber")] public string? TaxRegistrationNumber { get; set; }
    }
}

