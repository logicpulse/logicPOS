using ErrorOr;
using LogicPOS.Api.Features.Articles.Common;
using MediatR;
using System.Collections.Generic;
using System;

namespace LogicPOS.Api.Features.Articles
{
    public class AddArticlesStockCommand : IRequest<ErrorOr<Unit>>
    {
        public Guid SupplierId { get; set; }
        public string Date { get; set; } 
        public string DocumentNumber { get; set; }
        public string Notes { get; set; }
        public IEnumerable<ArticleStock> Articles { get; set; } 
    }
}
