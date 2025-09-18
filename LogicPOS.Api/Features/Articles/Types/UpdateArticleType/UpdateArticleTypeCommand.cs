using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Types.UpdateArticleType
{
    public class UpdateArticleTypeCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint NewOrder { get; set; }
        public string NewCode { get; set; }
        public string NewDesignation { get; set; }
        public bool HasPrice {  get; set; }
        public string NewNotes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
