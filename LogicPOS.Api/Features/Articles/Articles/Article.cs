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
        public ArticleClass Class { get; set; }
        public Guid ClassId { get; set; }

        public ArticleSubfamily Subfamily { get; set; }
        public Guid SubfamilyId { get; set; }

        public ArticleType Type { get; set; }
        public Guid TypeId { get; set; }

        public MeasurementUnit MeasurementUnit { get; set; }
        public Guid MeasurementUnitId { get; set; }

        public SizeUnit SizeUnit { get; set; }
        public Guid SizeUnitId { get; set; }

        public CommissionGroup CommissionGroup { get; set; }
        public Guid? CommissionGroupId { get; set; }

        public DiscountGroup DiscountGroup { get; set; }
        public Guid? DiscountGroupId { get; set; }

        public VatRate VatOnTable { get; set; }
        public Guid? VatOnTableId { get; set; }

        public VatRate VatDirectSelling { get; set; }
        public Guid VatDirectSellingId { get; set; }

        public VatExemptionReason VatExemptionReason { get; set; }
        public Guid? VatExemptionReasonId { get; set; }

        public Printer Printer { get; set; }
        public Guid? PrinterId { get; set; }
        #endregion

        #region Properties
        public string CodeDealer { get; set; }
        public Button Button { get; set; }
        public ArticlePrice Price1 { get; set; }
        public ArticlePrice Price2 { get; set; }
        public ArticlePrice Price3 { get; set; }
        public ArticlePrice Price4 { get; set; }
        public ArticlePrice Price5 { get; set; }
        public bool PriceWithVat { get; set; }
        public decimal? Discount { get; set; }
        public uint? DefaultQuantity { get; set; }
        public decimal? Accounting { get; set; }
        public uint? MinimumStock { get; set; }
        public decimal? Tare { get; set; }
        public float? Weight { get; set; }
        public string Barcode { get; set; }
        public bool PVPVariable { get; set; }
        public bool Favorite { get; set; }
        public bool UseWeighingBalance { get; set; }
        public string Token1 { get; set; }
        public string Token2 { get; set; }
        public string TemplateBarcode { get; set; }
        public bool IsComposed { get; set; }
        public bool UniqueArticles { get; set; }
        #endregion
    }
}
