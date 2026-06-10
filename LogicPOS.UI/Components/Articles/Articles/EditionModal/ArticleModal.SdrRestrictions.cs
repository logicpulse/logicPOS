namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleModal
    {
        private void ApplySdrDepositArticleFieldRestrictions()
        {
            if (_modalMode == EntityEditionModalMode.Insert || _entity == null)
            {
                return;
            }

            if (!LogicPOS.Api.Features.Articles.Common.SdrConstants.IsDepositArticle(_entity.Code))
            {
                return;
            }

            _txtCode.Component.Sensitive = false;
            _txtDesignation.Component.Sensitive = false;
            _comboClasses.ComboBox.Sensitive = false;
            _comboVatDirectSelling.ComboBox.Sensitive = false;
            _comboVatExemptionReasons.ComboBox.Sensitive = false;
            _checkPriceWithVat.Sensitive = false;
            _txtDiscount.Component.Sensitive = false;
            _checkIsComposed.Sensitive = false;
            _checkUniqueArticles.Sensitive = false;
            _checkIsSdrPackaging.Sensitive = false;
            _txtMinimumStock.Component.Sensitive = false;
            _checkUseWeighingBalance.Sensitive = false;
            _checkPVPVariable.Sensitive = false;
            _checkDisabled.Sensitive = false;

            foreach (var priceField in _prices)
            {
                priceField.Component.Sensitive = false;
            }
        }
    }
}
