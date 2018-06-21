using logicpos.datalayer.Enums;
using System;

namespace logicpos.Classes.Logic.Others
{
    class SplitPaymentArticleComparable : IComparable
    {
        public Guid oid;
        public string designation;
        public decimal priceFinal;
        public decimal price;
        public decimal vat;
        public Guid vatExemptionReason;
        public Guid placeOid;
        public Guid tableOid;
        public PriceType priceType;

        public SplitPaymentArticleComparable(Guid oid, string designation, decimal priceFinal, decimal price, decimal vat, Guid vatExemptionReason, Guid placeOid, Guid tableOid, PriceType priceType)
        {
            this.oid = oid;
            this.designation = designation;
            // Used Split Values with FinalTotal
            this.priceFinal = priceFinal;
            // Used to send to ArticleBag
            this.price = price;
            this.vat = vat;
            this.vatExemptionReason = vatExemptionReason;
            this.placeOid = placeOid;
            this.tableOid = tableOid;
            this.priceType = priceType;
        }

        public override string ToString()
        {
            return $"{designation} > {price}";
        }

        // This function is used to sorts the array list, based on price
        public int CompareTo(Object obj)
        {
            if (obj is SplitPaymentArticleComparable)
            {
                // return ascending or descending based
                return this.price < (obj as SplitPaymentArticleComparable).price ? -1 : 1;
            }
            else
            {
                // equal
                return 0;
            }
        }
    }
}