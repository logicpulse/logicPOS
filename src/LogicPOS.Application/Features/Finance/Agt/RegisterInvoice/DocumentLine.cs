
using LogicPOS.Application.Features.Finance.Agt.Common;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.RegisterInvoice;

public record DocumentLine
{
    [JsonProperty("lineNumber")] public string LineNumber { get; set; } = null!;

    [JsonProperty("productCode")] public string ProductCode { get; set; } = null!;

    [JsonProperty("productDescription")]
    public string ProductDescription { get; set; } = null!;

    [JsonProperty("quantity")] public string Quantity { get; set; } = null!;

    [JsonProperty("unitOfMeasure")] public string? UnitOfMeasure { get; set; }

    [JsonProperty("unitPrice")] public string? UnitPrice { get; set; }

    [JsonProperty("unitPriceBase")] public string? UnitPriceBase { get; set; }

    [JsonProperty("referenceInfo")] public ReferenceInfo? ReferenceInfo { get; set; } 

    [JsonProperty("debitAmount")] public string? DebitAmount { get; set; }

    [JsonProperty("creditAmount")] public string? CreditAmount { get; set; }

    [JsonProperty("taxes")] public List<DocumentLineTax>? Taxes { get; set; }

    [JsonProperty("settlementAmount")] public string SettlementAmount { get; set; } = "0";
}