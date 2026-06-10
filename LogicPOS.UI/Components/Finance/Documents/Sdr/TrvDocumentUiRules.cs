using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Finance.PaymentMethods;
using System;

namespace LogicPOS.UI.Components.Finance.Documents.Sdr
{
    public static class TrvDocumentUiRules
    {
        public const string DocumentTypeAcronym = "TRV";
        public const string PaymentMethodAcronym = "OU";

        public static bool IsTrvDocument(string documentType)
            => string.Equals(documentType, DocumentTypeAcronym, StringComparison.OrdinalIgnoreCase);

        public static bool IsAllowedArticle(string articleCode)
            => SdrConstants.IsDepositArticle(articleCode);

        public static PaymentMethod ResolvePaymentMethod()
            => PaymentMethodsService.GetByAcronym(PaymentMethodAcronym);

        public static ArticleViewModel ResolveDepositArticle()
            => ArticlesService.GetArticleByCode(SdrConstants.SdrArticleCode);
    }
}
