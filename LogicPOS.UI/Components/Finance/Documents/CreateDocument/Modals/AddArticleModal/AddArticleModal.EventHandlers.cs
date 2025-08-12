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
                Item.Article = TxtArticle.SelectedEntity as Article;
                Item.ArticleId = (TxtArticle.SelectedEntity as Article)?.Id ?? Item.ArticleId;
                Item.Code = Item.Article?.Code ?? Item.Code;
                Item.Designation = TxtArticle?.Text ?? Item.Designation;
                Item.UnitPrice = decimal.Parse(TxtPrice.Text);
                Item.Quantity = decimal.Parse(TxtQuantity.Text);
                Item.Discount = decimal.Parse(TxtDiscount.Text);
                Item.VatRate = TxtTax.SelectedEntity as VatRate;
                Item.VatRateId = (TxtTax.SelectedEntity as VatRate)?.Id ?? Item.VatRateId;
                Item.VatDesignation = TxtTax.Text;
                Item.Vat = _vatRateValue;
                Item.VatExemptionReason = TxtVatExemptionReason.SelectedEntity as VatExemptionReason;
                Item.ExemptionReason = Item.VatExemptionReason is null ? TxtVatExemptionReason.Text : Item.VatExemptionReason.Designation;
                Item.Notes = TxtNotes.Text;
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
                var article = page.GetSelectedArticle();
                SelectArticle(article);
            }
        }

        private void TxtCode_OnCompletionSelected(object obj)
        {
            ArticleViewModel articleViewModel = obj as ArticleViewModel;
            var article = ArticlesService.GetArticlebById(articleViewModel.Id);
            SelectArticle(article);
        }

    }
}
