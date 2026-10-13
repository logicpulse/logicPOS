namespace LogicPOS.Domain.Entities.Finance.Calculations;

/// <summary>
/// Converts shelf or client prices to net unit prices when PriceIncludesVat on the detail DTO
/// or the article's PriceWithVat flag is set (R7). Prefer DocumentLineCalculator.ToNetUnitPrice at call sites.
/// </summary>
public static class UnitPriceCalculator
{
    public static decimal ToNetUnitPrice(decimal price, decimal vatPercentage, bool priceWithVat)
    {
        if (!priceWithVat || vatPercentage <= 0)
        {
            return price;
        }

        return price / (1 + vatPercentage / 100m);
    }
}
