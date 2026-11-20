namespace LogicPOS.Domain.Entities.Finance.Calculations;

/// <summary>
/// Maps stored net unit prices back to UI columns when a document line is reopened.
/// </summary>
public static class DocumentReopenedLinePrice
{
    public readonly record struct Display(decimal UnitPrice, decimal NetUnitPrice, bool PriceIncludesVat);

    public static Display FromStoredNet(decimal storedNetUnitPrice, decimal vatPercentage, bool priceIncludesVat)
    {
        if (!priceIncludesVat || vatPercentage <= 0)
        {
            return new Display(storedNetUnitPrice, storedNetUnitPrice, false);
        }

        var unitPrice = (storedNetUnitPrice * (1 + vatPercentage / 100m)).ToStoredPrecision();
        var netUnitPrice = DocumentLineCalculator.ToNetUnitPrice(unitPrice, vatPercentage, priceWithVat: true);
        return new Display(unitPrice, netUnitPrice, true);
    }
}