using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleModal
    {
        private bool IsSdrDepositArticle =>
            _entity != null && SdrConstants.IsDepositArticle(_entity.Code);

        private static EntityEditionModalMode ResolveModalMode(EntityEditionModalMode modalMode, Article entity)
        {
            if (entity != null && SdrConstants.IsDepositArticle(entity.Code))
            {
                return EntityEditionModalMode.View;
            }

            return modalMode;
        }

        private void ApplySdrDepositArticleUi()
        {
            if (!IsSdrDepositArticle)
            {
                return;
            }

            _pricesArea.Visible = false;
            _sdrDepositPriceArea.Visible = true;
            _checkPVPVariable.Visible = false;
            _checkPriceWithVat.Visible = false;
            _checkIsSdrPackaging.Visible = false;
            _txtDiscount.Component.Visible = false;
        }
    }
}
