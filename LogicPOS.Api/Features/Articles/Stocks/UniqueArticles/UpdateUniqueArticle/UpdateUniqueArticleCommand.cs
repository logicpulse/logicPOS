using ErrorOr;
using MediatR;
using System.Collections.Generic;
using System;

namespace LogicPOS.Api.Features.Articles.StockManagement.UpdateUniqueArticle
{
    public class UpdateUniqueArticleCommand : IRequest<ErrorOr<Success>>
    {
        public Guid Id { get; set; }
        public string SerialNumber { get; set; }
        public IEnumerable<Guid> ChildUniqueArticles { get; set; }
    }
}
