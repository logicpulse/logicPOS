using LogicPOS.Api.Features.Articles;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.ValueObjects;
using System;

namespace LogicPOS.Api.Entities
{
    public class Article : ApiEntity, IWithCode, IWithDesignation
    {
        public string Code { get; set; }
        public uint Order { get; set; }
        public string Designation { get; set; }

        #region  Dependencies
        public Guid ClassId { get; set; }
        public ArticleSubfamily Subfamily { get; set; }
        public Guid SubfamilyId { get; set; }
        public ArticleType Type { get; set; }
        public MeasurementUnit MeasurementUnit { get; set; }
        public SizeUnit SizeUnit { get; set; }
        public CommissionGroup CommissionGroup { get; set; }
        public DiscountGroup DiscountGroup { get; set; }
        public VatRate VatDirectSelling { get; set; }
        public VatExemptionReason VatExemptionReason { get; set; }
 
        #endregion

        #region Properties

        public Button Button { get; set; }
        public ArticlePrice Price1 { get; set; }
        public ArticlePrice Price2 { get; set; }
        public ArticlePrice Price3 { get; set; }
        public ArticlePrice Price4 { get; set; }
        public ArticlePrice Price5 { get; set; }
        public bool PriceWithVat { get; set; }
        public decimal Discount { get; set; }
        public decimal DefaultQuantity { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal Tare { get; set; }
        public float Weight { get; set; }
        public string Barcode { get; set; }
        public bool PVPVariable { get; set; }
        public bool Favorite { get; set; }
        public bool UseWeighingBalance { get; set; }
        public bool IsComposed { get; set; }
        public bool UniqueArticles { get; set; }
        #endregion
    }
}
