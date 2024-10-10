using ErrorOr;
using LogicPOS.Api.ValueObjects;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Subfamilies.UpdateArticleSubfamily
{
    public class UpdateArticleSubfamilyCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public uint? NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public Guid? NewFamilyId { get; set; }
        public Guid? NewCommissionGroupId { get; set; }
        public Guid? NewDiscountGroupId { get; set; }
        public Guid? NewVatOnTableId { get; set; }
        public Guid? NewVatDirectSellingId { get; set; }
        public Button NewButton { get; set; }
        public string NewNotes { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
