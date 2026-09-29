using ErrorOr;
using LogicPOS.Api.ValueObjects;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Subfamilies.UpdateArticleSubfamily
{
    public class UpdateArticleSubfamilyCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public Guid FamilyId { get; set; }
        public Guid? CommissionGroupId { get; set; }
        public Guid? DiscountGroupId { get; set; }
        public Guid? VatOnTableId { get; set; }
        public Guid? VatDirectSellingId { get; set; }
        public Button Button { get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
