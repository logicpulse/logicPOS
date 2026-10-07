

using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.RegisterInvoice;

public record PaymentReceiptSourceDocument
{
    [JsonProperty("lineNo")] public string LineNo { get; set; } = null!;

    [JsonProperty("sourceDocumentID")] public SourceDocumentId SourceDocumentId { get; set; } = null!;

    [JsonProperty("debitAmount")] public string? DebitAmount { get; set; }

    [JsonProperty("creditAmount")] public string? CreditAmount { get; set; }
}