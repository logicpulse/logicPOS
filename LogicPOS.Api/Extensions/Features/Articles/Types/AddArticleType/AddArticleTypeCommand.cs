using ErrorOr;
using MediatR;
using System;

namespace LogicPOS.Api.Features.Articles.Types.AddArticleType
{
    public class AddArticleTypeCommand : IRequest<ErrorOr<Guid>>
    {
        public string Designation { get; set; }
        public bool HasPrice {  get; set; }
        public string Notes { get; set; }
    }
}
