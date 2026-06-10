using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.Sdr;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using DocumentDetail = LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument.DocumentDetail;

namespace LogicPOS.UI.Components.Finance.Documents.Sdr
{
    public static class SdrDocumentDetailsService
    {
        public static List<DocumentDetail> Enrich(IEnumerable<DocumentDetail> details)
        {
            var list = details.ToList();
            var depositArticle = ResolveDepositArticle();
            var packagingMap = BuildPackagingMap(list);

            return DocumentDetailSdrEnricher.Enrich(list, packagingMap, depositArticle);
        }

        public static List<DocumentDetail> EnrichFromSaleItems(IEnumerable<SaleItem> items)
        {
            var compactedItems = SaleItem.Compact(items).ToList();
            var details = SaleItem.GetOrderDetailsFromSaleItems(compactedItems).ToList();
            var depositArticle = ResolveDepositArticle();
            var packagingMap = BuildPackagingMapFromSaleItems(compactedItems);

            return DocumentDetailSdrEnricher.Enrich(details, packagingMap, depositArticle);
        }

        public static List<SaleItem> EnrichSaleItemsForDisplay(IEnumerable<SaleItem> items, bool compactItems = false)
        {
            var list = (compactItems ? SaleItem.Compact(items) : items)
                .Where(i => !SdrConstants.IsDepositArticle(i.Article.Code))
                .ToList();

            var depositArticle = ResolveDepositArticle();
            if (depositArticle == null)
            {
                return list;
            }

            var packagingUnits = list
                .Where(i => i.Article.IsSdrPackaging)
                .Sum(i => i.Quantity);

            if (packagingUnits <= 0)
            {
                return list;
            }

            list.Add(SaleItem.CreateDepositLine(depositArticle, packagingUnits));
            return list;
        }

        public static decimal CalculateDepositTotal(IEnumerable<SaleItem> items)
        {
            var depositArticle = ResolveDepositArticle();
            var packagingItems = items.Select(item => new SdrPackagingSaleItem
            {
                ArticleCode = item.Article.Code,
                IsSdrPackaging = item.Article.IsSdrPackaging,
                Quantity = item.Quantity
            });

            return SdrDepositAmountCalculator.Calculate(packagingItems, depositArticle);
        }

        private static ArticleViewModel ResolveDepositArticle()
            => ArticlesService.GetArticleByCode(SdrConstants.SdrArticleCode);

        private static Dictionary<Guid, bool> BuildPackagingMap(IEnumerable<DocumentDetail> details)
        {
            return details
                .Select(d => d.ArticleId)
                .Distinct()
                .ToDictionary(id => id, ResolveIsSdrPackaging);
        }

        private static Dictionary<Guid, bool> BuildPackagingMapFromSaleItems(IEnumerable<SaleItem> items)
            => items
                .GroupBy(i => i.Article.Id)
                .ToDictionary(g => g.Key, g => g.First().Article.IsSdrPackaging);

        private static bool ResolveIsSdrPackaging(Guid articleId)
        {
            var article = ArticlesService.GetArticleViewModel(articleId);
            if (article == null || SdrConstants.IsDepositArticle(article.Code))
            {
                return false;
            }

            return article.IsSdrPackaging;
        }
    }
}
