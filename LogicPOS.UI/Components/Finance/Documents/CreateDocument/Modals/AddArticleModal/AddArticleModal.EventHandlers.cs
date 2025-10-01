using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using MediatR;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddArticleModal
    {

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            Validate();

            if (AllFieldsAreValid() == false)
            {
                return;
            }

            if (_mode == EntityEditionModalMode.Update)
            {
                DocumentDetail.Article = TxtArticle.SelectedEntity as ArticleViewModel;
                DocumentDetail.ArticleId = (TxtArticle.SelectedEntity as ArticleViewModel)?.Id ?? DocumentDetail.ArticleId;
                DocumentDetail.Code = DocumentDetail.Article?.Code ?? DocumentDetail.Code;
                DocumentDetail.Designation = TxtArticle?.Text ?? DocumentDetail.Designation;
                DocumentDetail.UnitPrice = decimal.Parse(TxtPrice.Text);
                DocumentDetail.Quantity = decimal.Parse(TxtQuantity.Text);
                DocumentDetail.Discount = decimal.Parse(TxtDiscount.Text);
                DocumentDetail.VatRate = TxtTax.SelectedEntity as VatRate;
                DocumentDetail.VatRateId = (TxtTax.SelectedEntity as VatRate)?.Id ?? DocumentDetail.VatRateId;
                DocumentDetail.VatDesignation = TxtTax.Text;
                DocumentDetail.Vat = _vatRateValue;
                DocumentDetail.VatExemptionReason = TxtVatExemptionReason.SelectedEntity as VatExemptionReason;
                DocumentDetail.ExemptionReason = DocumentDetail.VatExemptionReason is null ? TxtVatExemptionReason.Text : DocumentDetail.VatExemptionReason.Designation;
                DocumentDetail.Notes = TxtNotes.Text;
            }
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            Clear();
            Run();
        }

        private void BtnSelectVatExemptionReason_Clicked(object sender, EventArgs e)
        {
            var page = new VatExemptionReasonsPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<VatExemptionReason>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtVatExemptionReason.Text = page.SelectedEntity.Designation;
                TxtVatExemptionReason.SelectedEntity = page.SelectedEntity;
            }
        }

        private void BtnSelectTax_Clicked(object sender, EventArgs e)
        {
            var page = new VatRatesPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<VatRate>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtTax.Text = page.SelectedEntity.Designation;
                TxtTax.SelectedEntity = page.SelectedEntity;
                _vatRateValue = page.SelectedEntity.Value;
                UpdateTotals();
                UpdateValidatableFields();
            }
        }

        private void TxtCode_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtCode.Text))
            {
                Clear();
            }
        }

        private void TxtArticle_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtArticle.Text))
            {
                Clear();
            }
        }

        private void BtnSelectArticle_Clicked(object sender, EventArgs e)
        {
            var page = new ArticlesPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<ArticleViewModel>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                SelectArticle(page.SelectedEntity);
            }
        }

        private void TxtCode_OnCompletionSelected(object obj)
        {
            SelectArticle(obj as ArticleViewModel);
        }

    }
}
