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
            Vat = article.VatDirectSelling?.Value ?? 0;
            Discount = article.Discount;
        }

        public SaleItem(OrderDetail detail)
        {
            Article = detail.Article;
            UnitPrice = detail.Price;
            Quantity = detail.Quantity;
            Vat = detail.Vat;
            Discount = detail.Discount;
        }

        public SaleItem()
        {
        }

        public Article Article { get; set; }
        public decimal Discount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalFinal => TotalNet + VatPrice;
        public decimal TotalNet => Quantity * UnitPrice - DiscountPrice;
        public decimal DiscountPrice => Quantity * UnitPrice * Discount / 100;
        public decimal VatPrice => TotalNet * Vat / 100;

    }
}
