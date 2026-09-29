using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Types.UpdateArticleType
{
    public class UpdateArticleTypeCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public uint Order { get; set; }
        public string Code { get; set; }
        public string Designation { get; set; }
        public bool HasPrice {  get; set; }
        public string Notes { get; set; }
        public bool IsDeleted { get; set; }
    }
}
