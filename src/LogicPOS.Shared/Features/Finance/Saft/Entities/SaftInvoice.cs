using LogicPOS.Shared.Extensions;

namespace LogicPOS.Shared.Features.Finance.Saft.Entities;

public record SaftInvoice : SaftDocument
{
    public decimal TotalDelivery { get; set; }
    public decimal TotalChange { get; set; }
    public new decimal TaxPayable => Lines.Sum(line => line.TaxPayable);
    public decimal SettlementAmount { get; set; }
    
    public IEnumerable<SaftPaymentMechanism>? PaymentMechanisms { get; set; }

}