namespace LogicPOS.Api.Features.Articles
{
    public class ArticlePrice
    {
        public decimal Value { get; set; }
        public decimal PromotionValue { get; set; }
        public bool UsePromotion { get; set; }
    }
}
