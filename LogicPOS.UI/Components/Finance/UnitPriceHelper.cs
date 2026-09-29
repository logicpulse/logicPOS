using LogicPOS.Api.Features.Articles.Common;

namespace LogicPOS.UI.Components.Finance
{
    /// <summary>
    /// Converts catalog or entered prices to net unit prices for document line calculations.
    /// Document lines always store and calculate from net unit price.
    /// </summary>
    public static class UnitPriceHelper
    {
        public static decimal ToNetUnitPrice(decimal price, decimal vatPercentage, bool priceWithVat)
        {
            if (!priceWithVat || vatPercentage <= 0)
            {
                return price;
            }

            return price / (1 + vatPercentage / 100M);
        }

        public static decimal ToNetUnitPrice(decimal price, decimal vatPercentage, ArticleViewModel article)
            => ToNetUnitPrice(price, vatPercentage, article?.PriceWithVat == true);

        public static decimal ParseUnitPrice(string priceText)
        {
            return decimal.TryParse(priceText, out var price) ? price : 0;
        }
    }
}
