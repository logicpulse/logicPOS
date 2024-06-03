using LogicPOS.Settings.Enums;
using LogicPOS.Settings;
using logicpos.shared.Enums;
using LogicPOS.Shared.Orders;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;

namespace LogicPOS.Shared.Article
{
    public static class ArticleUtils
    {
        public static PriceProperties GetArticlePrice(fin_article pArticle, TaxSellType pTaxSellType)
        {
            OrderMain orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
            return GetArticlePrice(pArticle, orderMain.Table.PriceType, pTaxSellType);
        }

        public static PriceProperties GetArticlePrice(fin_article pArticle, PriceType pPriceType, TaxSellType pTaxSellType)
        {
            decimal priceSource = 0.0m;
            decimal priceDefault = 0.0m;
            decimal priceTax = 0.0m;

            // Get priceTax Based on AppOperationMode : in retail mode VatOnTable is always null
            if (AppOperationModeSettings.AppMode == AppOperationMode.Default)
            {
                //Protecções de integridade das BD's e funcionamento da aplicação [IN:013327]
                // Default : Restaurants with dual Tax ex Normal, TakeAway
                if (pTaxSellType == TaxSellType.Normal && pArticle.VatOnTable != null) priceTax = pArticle.VatOnTable.Value;
                else if (pArticle.VatDirectSelling != null) priceTax = pArticle.VatDirectSelling.Value;
            }
            else if (AppOperationModeSettings.AppMode == AppOperationMode.Retail)
            {
                // Mono priceTax 
                if (pArticle.VatDirectSelling != null)
                    priceTax = pArticle.VatDirectSelling.Value;
            }

            //Default Price, used when others are less or equal to zero
            if (pArticle.Price1UsePromotionPrice && pArticle.Price1Promotion > 0)
            {
                priceDefault = pArticle.Price1Promotion;
            }
            else if (pArticle.Price1 > 0)
            {
                priceDefault = pArticle.Price1;
            }

            if (pArticle != null)
            {
                switch (pPriceType)
                {
                    case PriceType.Price1:
                        priceSource = priceDefault;
                        break;
                    case PriceType.Price2:
                        if (pArticle.Price2UsePromotionPrice && pArticle.Price2Promotion > 0)
                        {
                            priceSource = pArticle.Price2Promotion;
                        }
                        else if (pArticle.Price2 > 0)
                        {
                            priceSource = pArticle.Price2;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                    case PriceType.Price3:
                        if (pArticle.Price3UsePromotionPrice && pArticle.Price3Promotion > 0)
                        {
                            priceSource = pArticle.Price3Promotion;
                        }
                        else if (pArticle.Price3 > 0)
                        {
                            priceSource = pArticle.Price3;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                    case PriceType.Price4:
                        if (pArticle.Price4UsePromotionPrice && pArticle.Price4Promotion > 0)
                        {
                            priceSource = pArticle.Price4Promotion;
                        }
                        else if (pArticle.Price4 > 0)
                        {
                            priceSource = pArticle.Price4;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                    case PriceType.Price5:
                        if (pArticle.Price5UsePromotionPrice && pArticle.Price5Promotion > 0)
                        {
                            priceSource = pArticle.Price5Promotion;
                        }
                        else if (pArticle.Price5 > 0)
                        {
                            priceSource = pArticle.Price5;
                        }
                        if (priceSource <= 0.0m) priceSource = priceDefault;
                        break;
                }
            }

            PricePropertiesSourceMode sourceMode = (!pArticle.PriceWithVat) ? PricePropertiesSourceMode.FromPriceNet : PricePropertiesSourceMode.FromPriceUser;

            //Get Price
            PriceProperties priceProperties = PriceProperties.GetPriceProperties(
              sourceMode,
              pArticle.PriceWithVat,
              priceSource,
              1.0m,
              pArticle.Discount,
              POSSession.GetGlobalDiscount(),
              priceTax
            );

            //Return PriceProperties Object
            return priceProperties;
        }
    }
}
