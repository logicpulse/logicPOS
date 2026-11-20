using LogicPOS.Domain.Entities.Finance.Calculations;

namespace LogicPOS.Core.FrontOffice;

/// <summary>Validates payment portions for POS mixed settlement (pagamento misto).</summary>
public static class PosMixedPaymentValidation
{
    public const decimal Tolerance = 0.01m;

    public static string? ValidatePortions(decimal amountDue, IReadOnlyList<decimal> portions)
    {
        if (portions.Count == 0)
        {
            return "Indique pelo menos um método de pagamento.";
        }

        foreach (var portion in portions)
        {
            if (portion <= 0)
            {
                return "Cada parcela deve ser superior a zero.";
            }
        }

        var sum = portions.Sum().Round();
        var due = amountDue.Round();
        if (Math.Abs(sum - due) > Tolerance)
        {
            return
                $"A soma dos pagamentos ({MoneyFormat.Format(sum)}) deve ser igual ao total a liquidar ({MoneyFormat.Format(due)}).";
        }

        return null;
    }
}