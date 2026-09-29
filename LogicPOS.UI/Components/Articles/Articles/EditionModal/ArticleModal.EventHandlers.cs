using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ArticleModal
    {
        private void AddEventHandlers()
        {
            _checkIsComposed.Toggled += (sender, e) => UpdateCompositionTabVisibility();
            _checkPriceWithVat.Toggled += (sender, e) => UpdatePriceWithVatField();
        }

        private void ComboBox_VatDirectSelling_Changed(object sender, EventArgs e)
        {
            UpdateValidatableFields();

            UpdatePriceWithVatField();
        }

        private void UpdatePriceWithVatField()
        {
            if (_comboVatDirectSelling.SelectedEntity is null || _comboVatDirectSelling.SelectedEntity.Value <= 0)
            {
                _checkPriceWithVat.Active = false;
            }
        }

    }
}
