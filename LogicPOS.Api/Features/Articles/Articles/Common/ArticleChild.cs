using System;

namespace LogicPOS.Api.Features.Articles.Common
{
    public struct ArticleChild
    {
        public Guid ArticleId { get; set; }
        public decimal Quantity { get; set; }
    }
}
