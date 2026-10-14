using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.BackOffice;

/// <summary>Amount a receipt payment credits against an invoice. Never the invoice total when part was already paid.</summary>
public static class ReceiptCredit
{
    public static decimal Outstanding(decimal totalFinal, decimal alreadyPaid)
    {
        var due = (totalFinal - alreadyPaid).Round();
        return due > 0 ? due : 0m;
    }
}
