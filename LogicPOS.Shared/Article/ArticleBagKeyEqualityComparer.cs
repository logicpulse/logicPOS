using System.Collections.Generic;

namespace LogicPOS.Shared.Article
{
    public class ArticleBagKeyEqualityComparer : IEqualityComparer<ArticleBagKey>
    {
        bool IEqualityComparer<ArticleBagKey>.Equals(ArticleBagKey x, ArticleBagKey y)
        {
            return x.ArticleId == y.ArticleId
              && x.Designation == y.Designation
              && x.Price == y.Price
              && x.Discount == y.Discount
              && x.Vat == y.Vat
              && x.VatExemptionReasonOid == y.VatExemptionReasonOid
            ;
        }

        int IEqualityComparer<ArticleBagKey>.GetHashCode(ArticleBagKey obj)
        {
            int hashCode = $"{obj.ArticleId}{obj.Designation}{obj.Price}{obj.Discount}{obj.Vat}{obj.VatExemptionReasonOid}".GetHashCode();
            return hashCode;
        }
    }

}
