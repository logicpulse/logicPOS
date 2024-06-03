using DevExpress.Xpo;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using System;

namespace LogicPOS.Shared.Article
{
    public class ArticleBagProperties
    {
        private Guid _placeOid;
        public Guid PlaceOid
        {
            get { return _placeOid; }
            set { _placeOid = value; }
        }

        private Guid _tableOid;
        public Guid TableOid
        {
            get { return _tableOid; }
            set { _tableOid = value; }
        }

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

        [Size(50)]
        public string Reason { get; set; }

        [Size(255)]
        public string Token1 { get; set; }

        [Size(255)]
        public string Token2 { get; set; }

        [Size(255)]
        public string Notes { get; set; }

        [Size(255)]
        public string SerialNumber { get; set; }

        [Size(255)]
        public string Warehouse { get; set; }

        public object TreeIter { get; set; }

        public int ListIndex { get; set; }

        public ArticleBagProperties(Guid pPlaceOid, Guid pTableOid, PriceType pPriceType, string pCode, decimal pQuantity, string pUnitMeasure)
        {
            _placeOid = pPlaceOid;
            _tableOid = pTableOid;
            PriceType = pPriceType;
            Code = pCode;
            Quantity = pQuantity;
            UnitMeasure = pUnitMeasure;
        }
    }
}
