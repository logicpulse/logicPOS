using ErrorOr;
using LogicPOS.Api.ValueObjects;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.UpdateArticle
{
    public class UpdateArticleCommand: IRequest<ErrorOr<Unit>> 
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewCodeDealer { get; set; }
        public string NewDesignation { get; set; }
        public Button NewButton { get; set; }
        public ArticlePrice NewPrice1 { get; set; } 
        public ArticlePrice NewPrice2 { get; set; } 
        public ArticlePrice NewPrice3 { get; set; } 
        public ArticlePrice NewPrice4 { get; set; } 
        public ArticlePrice NewPrice5 { get; set; } 
        public bool NewPriceWithVat { get; set; }
        public decimal NewDiscount { get; set; }
        public uint NewDefaultQuantity { get; set; }
        public decimal NewTotalStock { get; set; }
        public uint NewMinimumStock { get; set; }
        public decimal NewTare { get; set; }
        public float NewWeight { get; set; }
        public string NewBarcode { get; set; }
        public bool NewPVPVariable { get; set; }
        public bool Favorite { get; set; } 
        public bool UseWeighingBalance { get; set; }
        public Guid? NewSubfamilyId { get; set; }
        public Guid? NewTypeId { get; set; }
        public Guid? NewClassId { get; set; }
        public Guid? NewMeasurementUnitId { get; set; }
        public Guid? NewSizeUnitId { get; set; }
        public Guid? NewCommissionGroupId { get; set; }
        public Guid? NewDiscountGroupId { get; set; }
        public Guid? NewVatOnTableId { get; set; }
        public Guid? NewVatDirectSellingId { get; set; }
        public Guid? NewVatExemptionReasonId { get; set; }
        public bool IsComposed { get; set; }
        public bool UniqueArticles { get; set; } 
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
