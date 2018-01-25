using System;
using System.Collections.Generic;

namespace logicpos.shared.Classes.Finance
{
    public class ArticleBagKey
    {
        Guid _articleOid;
        public Guid ArticleOid
        {
            get { return _articleOid; }
            set { _articleOid = value; }
        }

        string _designation;
        public string Designation
        {
            get { return _designation; }
            set { _designation = value; }
        }

        decimal _price;
        public decimal Price
        {
            get { return _price; }
            set { _price = value; }
        }

        decimal _discount;
        public decimal Discount
        {
            get { return _discount; }
            set { _discount = value; }
        }

        decimal _vat;
        public decimal Vat
        {
            get { return _vat; }
            set { _vat = value; }
        }

        Guid _vatExemptionReasonOid;
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
            _designation = pDesignation;
            _price = pPrice;
            _discount = pDiscount;
            _vat = pVat;
            _vatExemptionReasonOid = pVatExemptionReasonOid;
        }

        // IEqualityComparer Interface Implementation
        public class EqualityComparer : IEqualityComparer<ArticleBagKey>
        {
            bool IEqualityComparer<ArticleBagKey>.Equals(ArticleBagKey x, ArticleBagKey y)
            {
                return x._articleOid == y._articleOid
                  && x._designation == y._designation
                  && x._price == y._price
                  && x._discount == y._discount
                  && x._vat == y._vat
                  && x._vatExemptionReasonOid == y._vatExemptionReasonOid
                ;
            }

            int IEqualityComparer<ArticleBagKey>.GetHashCode(ArticleBagKey obj)
            {
                int hashCode = string.Format("{0}{1}{2}{3}{4}{5}", obj._articleOid, obj._designation, obj._price, obj._discount, obj._vat, obj._vatExemptionReasonOid).GetHashCode();
                return hashCode;
            }
        }
    }
}
