
using LogicPOS.Application.Features.Finance.Agt.RegisterInvoice;
using Newtonsoft.Json;

namespace LogicPOS.Application.Features.Finance.Agt.Common;

public record PaymentReceipt
{
    [JsonProperty("sourceDocuments")]
    public List<PaymentReceiptSourceDocument> SourceDocuments { get; set; } = null!;
}