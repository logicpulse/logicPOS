using LogicPOS.Shared.Extensions;

namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftInvoice : SaftDocument
{
    public decimal TotalDelivery { get; set; }
    public decimal TotalChange { get; set; }
    public decimal SettlementAmount { get; set; }
    public decimal WithholdingTaxAmount { get; set; }
    
    public IEnumerable<SaftPaymentMechanism>? PaymentMechanisms { get; set; }

}