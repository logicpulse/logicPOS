using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Api.Features.Finance.Documents.Documents.Sdr
{
    public static class DocumentDetailSdrEnricher
    {
        public static List<DocumentDetail> Enrich(
            List<DocumentDetail> details,
            IReadOnlyDictionary<Guid, bool> packagingByArticleId,
            ArticleViewModel depositArticle)
        {
            var withoutDeposit = RemoveDepositLines(details, depositArticle?.Id);
            if (depositArticle == null)
            {
                return withoutDeposit;
            }

            var packagingUnits = CountPackagingUnits(withoutDeposit, packagingByArticleId, depositArticle.Id);
            if (packagingUnits <= 0)
            {
                return withoutDeposit;
            }

            withoutDeposit.Add(CreateDepositLine(depositArticle, packagingUnits));
            return withoutDeposit;
        }

        private static List<DocumentDetail> RemoveDepositLines(List<DocumentDetail> details, Guid? depositArticleId)
        {
            if (depositArticleId == null)
            {
                return details.ToList();
            }

            return details.Where(d => d.ArticleId != depositArticleId.Value).ToList();
        }

        private static decimal CountPackagingUnits(
            IEnumerable<DocumentDetail> details,
            IReadOnlyDictionary<Guid, bool> packagingByArticleId,
            Guid depositArticleId)
        {
            decimal total = 0;

            foreach (var detail in details)
            {
                if (detail.ArticleId == depositArticleId)
                {
                    continue;
                }

                if (!packagingByArticleId.TryGetValue(detail.ArticleId, out var isPackaging) || !isPackaging)
                {
                    continue;
                }

                total += detail.Quantity;
            }

            return total;
        }

        private static DocumentDetail CreateDepositLine(ArticleViewModel depositArticle, decimal quantity)
            => new DocumentDetail
            {
                ArticleId = depositArticle.Id,
                Quantity = quantity,
                UnitPrice = depositArticle.Price1,
                VatRateId = depositArticle.VatRateId,
                VatExemptionId = depositArticle.VatExemptionReasonId,
                Discount = 0
            };
    }
}
