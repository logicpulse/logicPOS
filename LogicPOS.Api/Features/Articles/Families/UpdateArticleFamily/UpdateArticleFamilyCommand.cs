using ErrorOr;
using LogicPOS.Api.ValueObjects;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Families.UpdateArticleFamily
{
    public class UpdateArticleFamilyCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint? NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public Guid? NewCommissionGroupId { get; set; }
        public Button NewButton { get; set; }
        public string NewNotes { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
