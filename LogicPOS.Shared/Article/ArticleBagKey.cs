using System;

namespace LogicPOS.Shared.Article
{
    public partial class ArticleBagKey
    {
        public Guid ArticleId { get; set; }
        public string Designation { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Vat { get; set; }
        public Guid VatExemptionReasonId { get; set; }
 
        public ArticleBagKey(Guid articleId,
                             string designation,
                             decimal price,
                             decimal discount,
                             decimal vat,
                             Guid vatExemptionReasonId = new Guid())
        {
            ArticleId = articleId;
            Designation = designation;
            Price = price;
            Discount = discount;
            Vat = vat;
            VatExemptionReasonId = vatExemptionReasonId;
        }
    }
}
