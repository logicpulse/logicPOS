using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using System;

namespace LogicPOS.Shared.Article
{
    public class ArticleBagProperties
    {
        public Guid PlaceId { get; set; }
        public Guid TableId { get; set; }
        public PriceType PriceType { get; set; }
        public string Code { get; set; }
        public decimal Quantity { get; set; }
        public string UnitMeasure { get; set; }
        public decimal PriceWithDiscount { get; set; } = 0;
        public decimal PriceWithDiscountGlobal { get; set; } = 0;
        public decimal TotalGross { get; set; } = 0;
        public decimal TotalNet { get; set; } = 0;
        public decimal TotalDiscount { get; set; } = 0;
        public decimal TotalTax { get; set; } = 0;
        public decimal TotalFinal { get; set; } = 0;
        public decimal PriceFinal { get; set; } = 0;

        public fin_documentfinancemaster Reference { get; set; }

        public string Reason { get; set; }
        public string Token1 { get; set; }
        public string Token2 { get; set; }
        public string Notes { get; set; }
        public string SerialNumber { get; set; }
        public string Warehouse { get; set; }
        public object TreeIter { get; set; }
        public int ListIndex { get; set; }

        public ArticleBagProperties(Guid placeId,
                                    Guid tableId,
                                    PriceType priceType,
                                    string code,
                                    decimal quantity,
                                    string measurementUnit)
        {
            PlaceId = placeId;
            TableId = tableId;
            PriceType = priceType;
            Code = code;
            Quantity = quantity;
            UnitMeasure = measurementUnit;
        }
    }
}
