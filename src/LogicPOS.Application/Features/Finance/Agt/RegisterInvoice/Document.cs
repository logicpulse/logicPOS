
using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.RegisterInvoice;

public record Document
{
    [JsonProperty("documentNo")] public string DocumentNo { get; set; } = null!;

    [JsonProperty("documentStatus")] public string DocumentStatus { get; set; } = null!;

    [JsonProperty("documentCancelReason")]
    public string? DocumentCancelReason { get; set; }

    [JsonProperty("rejectedDocumentNo")]
    public string? RejectedDocumentNo { get; set; }

    [JsonProperty("jwsDocumentSignature")] public string JwsDocumentSignature { get; set; } = null!;

    [JsonProperty("documentDate")] public string DocumentDate { get; set; } = null!;

    [JsonProperty("documentType")] public string DocumentType { get; set; } = null!;

    [JsonProperty("eacCode")] public string? EacCode { get; set; }

    [JsonProperty("systemEntryDate")] public string SystemEntryDate { get; set; } = null!;

    [JsonProperty("customerTaxID")] public string? CustomerTaxId { get; set; }

    [JsonProperty("customerCountry")] public string? CustomerCountry { get; set; }

    [JsonProperty("companyName")] public string CompanyName { get; set; } = null!;

    [JsonProperty("lines")] public List<DocumentLine>? Lines { get; set; } 

    [JsonProperty("paymentReceipt")] public PaymentReceipt? PaymentReceipt { get; set; }

    [JsonProperty("documentTotals")] public DocumentTotals DocumentTotals { get; set; } = null!;

    [JsonProperty("withholdingTaxList")]
    public List<WithholdingTax>? WithholdingTaxList { get; set; } 
}