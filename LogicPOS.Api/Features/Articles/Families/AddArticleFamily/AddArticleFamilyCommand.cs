using ErrorOr;
using LogicPOS.Api.ValueObjects;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Families.AddArticleFamily
{
    public class AddArticleFamilyCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public Guid? CommissionGroupId { get; set; }
        public Guid? DiscountGroupId { get; set; }
        public Guid? PrinterId { get; set; }
        public Guid? PrinterTemplateId { get; set; }
        public Button Button { get; set; }
        public string Notes { get; set; }
    }
}
