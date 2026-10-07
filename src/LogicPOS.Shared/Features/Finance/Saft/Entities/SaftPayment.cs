using LogicPOS.Shared.Extensions;

namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftPayment
{
    public Guid Id { get; set; }
    public string PaymentRefNo { get; set; } = null!;
    public DateTime TransactionDate { get; set; } 
    public string PaymentType { get; set; } = null!;
    public SaftDocumentStatus Status { get; set; } = null!;
    public string PaymentMechanism { get; set; } = null!;
    public decimal GrossTotal { get; set; }
    public string PaymentDate { get; set; } = null!;
    public string SourceId { get; set; } = null!;
    public string SystemEntryDate { get; set; } = null!;
    public string CustomerId { get; set; } = null!;
    public List<SaftPaymentLine> Lines { get; set; } = [];
    public decimal TaxPayable { get; set; }
    public decimal NetTotal => GrossTotal - TaxPayable;
    public string? Atcud { get; set; }
}