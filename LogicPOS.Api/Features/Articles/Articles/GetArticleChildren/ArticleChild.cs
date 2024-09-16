using LogicPOS.Api.Entities;

namespace LogicPOS.Api.Features.Articles.GetArticleChildren
{
    public class ArticleChild
    {
        public Article Article { get; set; }
        public uint Quantity { get; set; }
    }
}
