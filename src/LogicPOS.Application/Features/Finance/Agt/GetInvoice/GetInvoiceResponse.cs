using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.GetInvoice;

public record GetInvoiceResponse
{
    [JsonProperty("documentNo")] public string DocumentNo { get; set; } = null!;
    [JsonProperty("documentStatus")] public string DocumentStatus { get; set; } = null!;
    
    [JsonProperty("document")]
    public GetInvoiceResponseDocument? Document { get; set; }
    
    [JsonProperty("errorList")] public List<ErrorListItem>? ErrorList { get; set; } 

    public record GetInvoiceResponseDocument
    {
        [JsonProperty("documentNo")] public string DocumentNo { get; set; } = null!;
        [JsonProperty("documentStatus")] public string DocumentStatus { get; set; } = null!;
        [JsonProperty("documentType")] public string DocumentType { get; set; } = null!;
        [JsonProperty("documentDate")] public string DocumentDate { get; set; } = null!;
        [JsonProperty("systemEntryDate")] public string SystemEntryDate { get; set; } = null!;
        [JsonProperty("customerTaxID")] public string CustomerTaxId { get; set; } = null!;
        [JsonProperty("customerCountry")] public string CustomerCountry { get; set; } = null!;
        [JsonProperty("companyName")] public string CompanyName { get; set; } = null!;
        [JsonProperty("emitterTaxId")] public string EmitterTaxId { get; set; } = null!;
        [JsonProperty("currencyCode")] public string CurrencyCode { get; set; } = null!;
        [JsonProperty("currencyAmount")] public string CurrencyAmount { get; set; } = null!;
        [JsonProperty("ExchangeRate")] public string ExchangeRate { get; set; } = null!;

        [JsonProperty("softwareValidationNo")]
        public string SoftwareValidationNo { get; set; } = null!;

        [JsonProperty("jwsSignature")] public string JwsSignature { get; set; } = null!;
        [JsonProperty("documentTotals")] public DocumentTotals DocumentTotals { get; set; } = new();

        [JsonProperty("documentItems")]
        public List<DocumentItem>? DocumentItems { get; set; } = [new DocumentItem()];
        
        
        [JsonProperty("holdingTaxes")] public List<WithholdingTax>? HoldingTaxes { get; set; } 

        [JsonProperty("paymentReceipts")]
        public List<PaymentReceipt>? PaymentReceipts { get; set; }

        public record DocumentItem
        {
            [JsonProperty("lineNo")] public string LineNo { get; set; } = null!;
            [JsonProperty("productCode")] public string ProductCode { get; set; } = null!;

            [JsonProperty("productDescription")]
            public string ProductDescription { get; set; } = null!;

            [JsonProperty("quantity")] public string Quantity { get; set; } = null!;
            [JsonProperty("unitOfMeasure")] public string? UnitOfMeasure { get; set; }
            [JsonProperty("unitPrice")] public string? UnitPrice { get; set; }
            [JsonProperty("unitPriceBase")] public string? UnitPriceBase { get; set; }
            [JsonProperty("debitAmount")] public string? DebitAmount { get; set; }
            [JsonProperty("creditAmount")] public string? CreditAmount { get; set; }
            [JsonProperty("settlementAmount")] public string? SettlementAmount { get; set; }
            [JsonProperty("taxes")] public List<DocumentLineTax>? Taxes { get; set; } = [new DocumentLineTax()];
        }
    }

    public record ErrorListItem
    {
        [JsonProperty("idError")] public string IdError { get; init; } = null!;
        [JsonProperty("descriptionError")] public string DescriptionError { get; init; } = null!;
    }
}