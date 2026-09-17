using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Documents.CreateDocument.Tabs.Details.AddArticleModal;
using LogicPOS.UI.Components.Finance.Documents.Sdr;
using LogicPOS.UI.Components.Licensing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddArticleModal
    {
        private readonly string _documentTypeAcronym;

        private bool IsTrvMode => TrvDocumentUiRules.IsTrvDocument(_documentTypeAcronym);

        private void ApplyTrvRestrictions()
        {
            if (!IsTrvMode)
            {
                return;
            }

            var depositArticle = TrvDocumentUiRules.ResolveDepositArticle();
            if (_mode == DocumentDetailModalMode.Insert)
            {
                if (depositArticle != null)
                {
                    SelectArticle(depositArticle);
                }
                else
                {
                    CustomAlerts.Warning(this)
                        .WithTitle("Artigo SDR em falta")
                        .WithMessage("Crie o artigo de depósito SDRVDEP antes de emitir o Talão Reembolso Volta.")
                        .ShowAlert();
                }
            }

            TxtArticle.Component.Sensitive = false;
            TxtArticle.BtnSelect.Sensitive = false;
            TxtCode.Component.Sensitive = false;
            TxtFamily.Component.Sensitive = false;
            TxtSubFamily.Component.Sensitive = false;
            TxtPrice.Component.Sensitive = false;
            TxtVatExemptionReason.Component.Sensitive = false;
            TxtVatRate.Component.Sensitive = false;
            TxtDiscount.Component.Sensitive = false;

            if (LicensingService.Data.StocksModule)
            {
                TxtSerialNumber.Component.Sensitive = false;
            }
        }

        private bool ValidateTrvArticleSelection()
        {
            if (!IsTrvMode)
            {
                return true;
            }

            var article = TxtArticle.SelectedEntity as ArticleViewModel;
            if (article != null && TrvDocumentUiRules.IsAllowedArticle(article.Code))
            {
                return true;
            }

            CustomAlerts.Warning(this)
                .WithTitle("Artigo inválido")
                .WithMessage("O Talão Reembolso Volta só permite o artigo de depósito SDR.")
                .ShowAlert();

            return false;
        }
    }
}
