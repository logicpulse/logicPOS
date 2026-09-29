using LogicPOS.Api.Features.Articles.Common;
using SdrConstants = LogicPOS.Api.Features.Articles.Common.SdrConstants;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Sdr
{
    public static class SdrDepositAmountCalculator
    {
        public static decimal Calculate(IEnumerable<SdrPackagingSaleItem> items, ArticleViewModel depositArticle)
        {
            if (depositArticle == null)
            {
                return 0;
            }

            var packagingUnits = items
                .Where(i => i.IsSdrPackaging && !SdrConstants.IsDepositArticle(i.ArticleCode))
                .Sum(i => i.Quantity);

            if (packagingUnits <= 0)
            {
                return 0;
            }

            return CalculateLineTotal(packagingUnits, depositArticle);
        }

        public static decimal CalculateLineTotal(decimal quantity, ArticleViewModel depositArticle)
        {
            var net = quantity * depositArticle.Price1;
            var vat = depositArticle.VatDirectSelling ?? 0;
            return net + net * vat / 100m;
        }
    }

    public sealed class SdrPackagingSaleItem
    {
        public string ArticleCode { get; set; }
        public bool IsSdrPackaging { get; set; }
        public decimal Quantity { get; set; }
    }
}
