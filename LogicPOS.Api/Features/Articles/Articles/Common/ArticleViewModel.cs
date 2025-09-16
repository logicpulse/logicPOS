using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.ValueObjects;
using System;

namespace LogicPOS.Api.Features.Articles.Common
{
    public class ArticleViewModel : ApiEntity, IWithCode, IWithDesignation
    {
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
        public decimal? VatDirectSelling { get; set; }
        public decimal Discount { get; set; }
        public string Unit { get; set; }
        public Guid SubfamilyId { get; set; }
        public Guid FamilyId { get; set; }
        public Button Button { get; set; }
        public bool PriceWithVat { get; set; }

        public decimal GetPrice(int priceType)
        {
            switch (priceType)
            {
                case 2:
                    return Price2;
                case 3:
                    return Price3;
                case 4:
                    return Price4;
                case 5:
                    return Price5;
                default:
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
