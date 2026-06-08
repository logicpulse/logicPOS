namespace LogicPOS.Api.Features.Articles
{
    public struct ArticlePrice
    {
        public decimal Value { get; set; }
        public decimal PromotionValue { get; set; }
        public bool UsePromotion { get; set; }
        public decimal Price => UsePromotion ? PromotionValue : Value;
    }
}
