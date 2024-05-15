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
        public Guid VatExemptionReasonOid { get; set; }
       

        public ArticleBagKey(Guid pArticleOid, string pDesignation, decimal pPrice, decimal pDiscount, decimal pVat)
            : this(pArticleOid, pDesignation, pPrice, pDiscount, pVat, new Guid())
        {
        }

        public ArticleBagKey(Guid pArticleOid, string pDesignation, decimal pPrice, decimal pDiscount, decimal pVat, Guid pVatExemptionReasonOid)
        {
            ArticleId = pArticleOid;
            Designation = pDesignation;
            Price = pPrice;
            Discount = pDiscount;
            Vat = pVat;
            VatExemptionReasonOid = pVatExemptionReasonOid;
        }
    }
}
