using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using System.Collections.Generic;
using System.Linq;
using DocumentDetailDto = LogicPOS.Api.Features.Documents.AddDocument.DocumentDetail;


namespace LogicPOS.UI.Components.POS
{
    public class SaleItem : IWithCode
    {
        public SaleItem(ArticleViewModel article)
        {
            Article = article;
            UnitPrice = article.Price;
            Quantity = article.DefaultQuantity > 0 ? article.DefaultQuantity : 1;
            Vat = article.VatDirectSelling ?? 0;
            Discount = article.Discount;
        }

        public SaleItem(OrderDetail detail)
        {
            Article = ArticleViewModel.FromEntity(detail.Article);
            UnitPrice = detail.Price;
            Quantity = detail.Quantity;
            Vat = detail.Vat;
            Discount = detail.Discount;
        }

        public SaleItem()
        {
        }

        public ArticleViewModel Article { get; set; }
        public decimal Discount { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalFinal => TotalNet + VatPrice;
        public decimal TotalNet => Quantity * UnitPrice - DiscountPrice;
        public decimal DiscountPrice => Quantity * UnitPrice * Discount / 100;
        public decimal VatPrice => TotalNet * Vat / 100;
        public string Code => Article.Code;

        public static IEnumerable<SaleItem> Uncompact(IEnumerable<SaleItem> items)
        {
            var singleItems = new List<SaleItem>();

            foreach (var item in items)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    singleItems.Add(new SaleItem
                    {
                        Article = item.Article,
                        Quantity = 1,
                        UnitPrice = item.UnitPrice,
                        Discount = item.Discount,
                        Vat = item.Vat
                    });
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
        
        public static IEnumerable<DocumentDetailDto> GetOrderDetailsFromSaleItems( IEnumerable<SaleItem> items)
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
