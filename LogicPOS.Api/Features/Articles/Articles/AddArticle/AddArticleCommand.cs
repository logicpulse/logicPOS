using ErrorOr;
using LogicPOS.Api.ValueObjects;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.AddArticle
{
    public class AddArticleCommand : IRequest<ErrorOr<Guid>>
    {
        public string Code { get; set; }
        public string CodeDealer { get; set; }
        public string Designation { get; set; } 
        public Button Button { get; set; }
        public ArticlePrice Price1 { get; set; }
        public ArticlePrice Price2 { get; set; }
        public ArticlePrice Price3 { get; set; }
        public ArticlePrice Price4 { get; set; }
        public ArticlePrice Price5 { get; set; }
        public bool PriceWithVat { get; set; }
        public decimal Discount { get; set; }
        public decimal DefaultQuantity { get; set; }
        public decimal TotalStock { get; set; }
        public decimal MinimumStock { get; set; }
        public decimal Tare { get; set; }
        public float Weight { get; set; }
        public string Barcode { get; set; }
        public bool PVPVariable { get; set; }
        public bool Favorite { get; set; } 
        public bool UseWeighingBalance { get; set; }
        public Guid SubfamilyId { get; set; }
        public Guid TypeId { get; set; }
        public Guid ClassId { get; set; }
        public Guid MeasurementUnitId { get; set; }
        public Guid SizeUnitId { get; set; } 
        public Guid? CommissionGroupId { get; set; }
        public Guid? DiscountGroupId { get; set; }
        public Guid? VatOnTableId { get; set; }
        public Guid VatDirectSellingId { get; set; } 
        public Guid? VatExemptionReasonId { get; set; }
        public bool IsComposed { get; set; } 
        public bool UniqueArticles { get; set; } 
        public string Notes { get; set; }
    }
}
