using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.POS.Orders.Orders.Common;
using LogicPOS.UI.Components.Articles;
using System.Collections.Generic;
using System.Linq;
using DocumentDetailDto = LogicPOS.Api.Features.Documents.AddDocument.DocumentDetail;


namespace LogicPOS.UI.Components.POS
{
    public class SaleItem : IWithCode
    {
        public SaleItem(ArticleViewModel article, string placePriceType)
        {
            Article = article;
            Vat = article.VatDirectSelling ?? 0;
            Quantity = article.DefaultQuantity > 0 ? article.DefaultQuantity : 1;
            Discount = article.Discount;
            switch(placePriceType)
            {
                case "20":
                    SetUnitPrice(article.Price2);
                    break;
                case "30":
                    SetUnitPrice(article.Price3);
                    break;
                case "40":
                    SetUnitPrice(article.Price4);
                    break;
                case "50":
                    SetUnitPrice(article.Price5);
                    break;
                default:
                    SetUnitPrice(article.Price1);
                    break;
            }
        }

        private SaleItem()
        {

        }

        public SaleItem(OrderDetail detail)
        {
            Article = detail.Article;
            Vat = detail.Vat;
            UnitPrice = detail.Price;
            Quantity = detail.Quantity;
            Discount = detail.Discount;
        }

        public ArticleViewModel Article { get; set; }
        public decimal Discount { get; set; }
        public decimal UnitPrice { get; private set; }
        public decimal Quantity { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalFinal => TotalNet + VatPrice;
        public decimal TotalNet => Quantity * UnitPrice - DiscountPrice;
        public decimal DiscountPrice => Quantity * UnitPrice * Discount;
        public decimal VatPrice => TotalNet * Vat / 100M;
        public string Code => Article.Code;
        public decimal UnitPriceWithVat => UnitPrice + (UnitPrice * (Vat / 100M));

        public void SetUnitPrice(decimal price)
        {
            UnitPrice = (Article.PriceWithVat && Vat > 0) ? ExtractPriceWithoutVat(price, Vat) : price;
        }

        public static decimal ExtractPriceWithoutVat(decimal priceWithVat, decimal vat)
        {
            return priceWithVat / (1 + vat / 100);
        }

        public SaleItem SingleClone()
        {
            return new SaleItem
            {
                Article = this.Article,
                Discount = this.Discount,
                UnitPrice = this.UnitPrice,
                Quantity = 1,
                Vat = this.Vat
            };
        }

        public static IEnumerable<SaleItem> Uncompact(IEnumerable<SaleItem> items)
        {
            var singleItems = new List<SaleItem>();

            foreach (var item in items)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    singleItems.Add(item.SingleClone());
                }
            }

            return singleItems;
        }

        public static IEnumerable<SaleItem> Compact(IEnumerable<SaleItem> items)
        {
            var orderItems = new List<SaleItem>();

            foreach (var item in items)
            {
                var existingItem = orderItems.FirstOrDefault(x => x.Article.Id == item.Article.Id &&
                                                             x.UnitPrice == item.UnitPrice);

                if (existingItem == null)
                {
                    orderItems.Add(new SaleItem
                    {
                        Article = item.Article,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        Vat = item.Vat
                    });
                }
                else
                {
                    existingItem.Quantity += item.Quantity;
                }
            }

            return orderItems;
        }

        public static IEnumerable<DocumentDetailDto> GetOrderDetailsFromSaleItems(IEnumerable<SaleItem> items)
        {
            items = Compact(items);

            var details = items.Select(item =>
            {
                return new DocumentDetailDto
                {
                    ArticleId = item.Article.Id,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount
                };
            });

            return details;
        }
    }
}
