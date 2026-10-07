namespace LogicPOS.Domain.Entities.Finance.Calculations;

/// <summary>
/// Converts catalog prices to net unit prices when articles are flagged as PriceWithVat.
/// Explicit prices sent by clients are always treated as net and are not converted here.
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
