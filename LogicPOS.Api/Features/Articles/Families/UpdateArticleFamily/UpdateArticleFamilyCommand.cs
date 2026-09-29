using ErrorOr;
using LogicPOS.Api.ValueObjects;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Families.UpdateArticleFamily
{
    public class UpdateArticleFamilyCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint? Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public Guid? CommissionGroupId { get; set; }
        public Button Button { get; set; }
        public string Notes { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
