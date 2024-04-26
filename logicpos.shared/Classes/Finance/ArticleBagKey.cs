using System;
using System.Collections.Generic;

namespace logicpos.shared.Classes.Finance
{
    public class ArticleBagKey
    {
        private Guid _articleOid;
        public Guid ArticleOid
        {
            get { return _articleOid; }
            set { _articleOid = value; }
        }

        public string Designation { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public decimal Vat { get; set; }

        private Guid _vatExemptionReasonOid;
        public Guid VatExemptionReasonOid
        {
            get { return _vatExemptionReasonOid; }
            set { _vatExemptionReasonOid = value; }
        }

        public ArticleBagKey(Guid pArticleOid, string pDesignation, decimal pPrice, decimal pDiscount, decimal pVat)
            : this(pArticleOid, pDesignation, pPrice, pDiscount, pVat, new Guid())
        {
        }

        public ArticleBagKey(Guid pArticleOid, string pDesignation, decimal pPrice, decimal pDiscount, decimal pVat, Guid pVatExemptionReasonOid)
        {
            _articleOid = pArticleOid;
            Designation = pDesignation;
            Price = pPrice;
            Discount = pDiscount;
            Vat = pVat;
            _vatExemptionReasonOid = pVatExemptionReasonOid;
        }

        // IEqualityComparer Interface Implementation
        public class EqualityComparer : IEqualityComparer<ArticleBagKey>
        {
            bool IEqualityComparer<ArticleBagKey>.Equals(ArticleBagKey x, ArticleBagKey y)
            {
                return x._articleOid == y._articleOid
                  && x.Designation == y.Designation
                  && x.Price == y.Price
                  && x.Discount == y.Discount
                  && x.Vat == y.Vat
                  && x._vatExemptionReasonOid == y._vatExemptionReasonOid
                ;
            }

            int IEqualityComparer<ArticleBagKey>.GetHashCode(ArticleBagKey obj)
            {
                int hashCode = string.Format("{0}{1}{2}{3}{4}{5}", obj._articleOid, obj.Designation, obj.Price, obj.Discount, obj.Vat, obj._vatExemptionReasonOid).GetHashCode();
                return hashCode;
            }
        }
    }
}
