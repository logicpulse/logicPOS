using ErrorOr;
using LogicPOS.Api.ValueObjects;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Families.UpdateArticleFamily
{
    public class UpdateArticleFamilyCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid Id { get; set; }
        public uint? NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public Guid? NewCommissionGroupId { get; set; }
        public Guid? NewDiscountGroupId { get; set; }
        public Guid? NewPrinterId { get; set; }
        public Button NewButton { get; set; }
        public string NewNotes { get; set; }
        public bool? IsDeleted { get; set; }

    }
}
