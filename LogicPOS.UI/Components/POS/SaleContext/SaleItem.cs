using LogicPOS.Api.Entities;

namespace LogicPOS.UI.Components.POS
{
    public class SaleItem
    {
        public SaleItem(Article article)
        {
            Article = article;
            UnitPrice = article.Price1.Price;
            Quantity = article.DefaultQuantity > 0 ? article.DefaultQuantity : 1;
        }

        public Article Article { get; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}
