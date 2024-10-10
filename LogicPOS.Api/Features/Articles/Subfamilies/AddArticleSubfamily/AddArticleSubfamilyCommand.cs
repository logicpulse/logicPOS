using ErrorOr;
using LogicPOS.Api.ValueObjects;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Subfamilies.AddArticleSubfamily
{
    public class AddArticleSubfamilyCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public Guid FamilyId { get; set; }
        public Guid? CommissionGroupId { get; set; }
        public Guid? DiscountGroupId { get; set; }
        public Guid? VatOnTableId { get; set; }
        public Guid? VatDirectSellingId { get; set; }
        public Button Button { get; set; }
        public string Notes { get; set; }
    }
}
