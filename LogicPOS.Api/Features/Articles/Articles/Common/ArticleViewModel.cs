using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.ValueObjects;
using System;

namespace LogicPOS.Api.Features.Articles.Common
{
    public class ArticleViewModel : ApiEntity, IWithCode, IWithDesignation
    {
        public uint Order { get; set; }
        public string Code { get; set; } 
        public bool IsComposed { get; set; }
        public string Family { get; set; } 
        public string Subfamily { get; set; } 
        public string Designation { get; set; } 
        public string Type { get; set; } 
        public decimal DefaultQuantity { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal Price3 { get; set; }
        public decimal Price4 { get; set; }
        public decimal Price5 { get; set; }

        public decimal Price1_PromotionValue { get; set; }
        public decimal Price2_PromotionValue { get; set; }
        public decimal Price3_PromotionValue { get; set; }
        public decimal Price4_PromotionValue { get; set; }
        public decimal Price5_PromotionValue { get; set; }

        public bool Price1_UsePromotion { get; set; }
        public bool Price2_UsePromotion { get; set; }
        public bool Price3_UsePromotion { get; set; }
        public bool Price4_UsePromotion { get; set; }
        public bool Price5_UsePromotion { get; set; }


        public decimal? VatDirectSelling { get; set; }
        public decimal Discount { get; set; }
        public string Unit { get; set; }
        public Guid SubfamilyId { get; set; }
        public Guid FamilyId { get; set; }
        public Guid VatRateId { get; set; }
        public Guid? VatExemptionReasonId { get; set; }
        public Button Button { get; set; }
        public bool PriceWithVat { get; set; }
        public string ClassAcronym { get; set; }

        public decimal GetPrice(int priceType)
        {
            switch (priceType)
            {
                case 2:
                    if(Price2_UsePromotion)
                    {
                        return Price2_PromotionValue;
                    }
                    return Price2;
                case 3:
                    if (Price3_UsePromotion)
                    {
                        return Price3_PromotionValue;
                    }
                    return Price3;
                case 4:
                    if (Price4_UsePromotion)
                    {
                        return Price4_PromotionValue;
                    }
                    return Price4;
                case 5:
                    if (Price5_UsePromotion)
                    {
                        return Price5_PromotionValue;
                    }
                    return Price5;
                default:
                    if (Price1_UsePromotion)
                    {
                        return Price1_PromotionValue;
                    }
                        return Price1;
            }
        }

        public void SetPrice(int priceType, decimal price)
        {
            switch (priceType)
            {
                case 2:
                    Price2 = price;
                    break;
                case 3:
                    Price3 = price;
                    break;
                case 4:
                    Price4 = price;
                    break;
                case 5:
                    Price5 = price;
                    break;
                default:
                    Price1 = price;
                    break;
            }
        }
    }
}
