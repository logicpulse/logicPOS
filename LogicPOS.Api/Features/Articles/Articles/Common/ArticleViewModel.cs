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

        public decimal Price1PromotionValue { get; set; }
        public decimal Price2PromotionValue { get; set; }
        public decimal Price3PromotionValue { get; set; }
        public decimal Price4PromotionValue { get; set; }
        public decimal Price5PromotionValue { get; set; }

        public bool Price1UsePromotion { get; set; }
        public bool Price2UsePromotion { get; set; }
        public bool Price3UsePromotion { get; set; }
        public bool Price4UsePromotion { get; set; }
        public bool Price5UsePromotion { get; set; }


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
        public string BarcodeLabelPrintModel { get; set; }
        public decimal GetPrice(int priceType)
        {
            switch (priceType)
            {
                case 2:
                    return Price2UsePromotion ? Price2PromotionValue : Price2;
                case 3:
                    return Price3UsePromotion ? Price3PromotionValue : Price3;
                case 4:
                    return Price4UsePromotion ? Price4PromotionValue : Price4;
                case 5:
                    return Price5UsePromotion ? Price5PromotionValue : Price5;
                default:
                    return Price1UsePromotion ? Price1PromotionValue : Price1;
            }
        }

        public void SetPrice(int priceType, decimal price)
        {
            switch (priceType)
            {
                case 2:
                    Price2PromotionValue = Price2UsePromotion ? price : Price2PromotionValue;
                    Price2 = Price2UsePromotion ? Price2 : price;
                    break;
                case 3:
                    Price3PromotionValue = Price3UsePromotion ? price : Price3PromotionValue;
                    Price3 = Price3UsePromotion ? Price3 : price;
                    break;
                case 4:
                    Price4PromotionValue = Price4UsePromotion ? price : Price4PromotionValue;
                    Price4 = Price4UsePromotion ? Price4 : price;
                    break;
                case 5:
                    Price5PromotionValue = Price5UsePromotion ? price : Price5PromotionValue;
                    Price5 = Price5UsePromotion ? Price5 : price;
                    break;
                default:
                    Price1PromotionValue = Price1UsePromotion ? price : Price1PromotionValue;
                    Price1 = Price1UsePromotion ? Price1 : price;
                    break;
            }
        }
    }
}
