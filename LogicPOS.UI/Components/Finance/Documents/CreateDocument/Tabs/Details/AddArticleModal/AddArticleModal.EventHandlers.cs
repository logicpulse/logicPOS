using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles;
using LogicPOS.Api.Features.Articles.AddArticle;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Components.ArticleClasses;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.ArticlesTypes;
using LogicPOS.UI.Components.MeasurementUnits;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.SizeUnits;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

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
            if(AllFieldsAreValid()==true && TxtArticle.SelectedEntity == null)
            {
                CreateArticleAndSelect();

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
                DocumentDetail.SerialNumber = TxtSerialNumber.Text;
            }
        }

        private void CreateArticleAndSelect()
        {
            var addArticleCommand = new AddArticleCommand()
            {
                Code = TxtCode.Text,
                Designation = TxtArticle.Text,
                PriceWithVat = true,
                Discount = decimal.Parse(TxtDiscount.Text),
                DefaultQuantity = 1,
                MinimumStock = 0,
                SubfamilyId = (TxtSubFamily.SelectedEntity as ArticleSubfamily).Id,
                TypeId = ArticleTypesService.DefaultArticleType.Id,
                ClassId = ArticleClassesService.DefaultArticleClass.Id,
                MeasurementUnitId = MeasurementUnitsService.DefaultMeasurementUnit.Id,
                SizeUnitId = SizeUnitsService.DefaultSizeUnit.Id,
                VatDirectSellingId = (TxtTax.SelectedEntity as VatRate).Id,
                VatExemptionReasonId = TxtVatExemptionReason.SelectedEntity == null ? null : (TxtVatExemptionReason.SelectedEntity as VatExemptionReason)?.Id,
                Notes = TxtNotes.Text,

                Price1 = new ArticlePrice()
                {
                    Value = decimal.Parse(TxtPrice.Text),
                    PromotionValue = 0,
                    UsePromotion = false,
                },
                Price2 = new ArticlePrice()
                {
                    Value = 0,
                    PromotionValue = 0,
                    UsePromotion = false,
                },
                Price3 = new ArticlePrice()
                {
                    Value = 0,
                    PromotionValue = 0,
                    UsePromotion = false,
                },
                Price4 = new ArticlePrice()
                {
                    Value = 0,
                    PromotionValue = 0,
                    UsePromotion = false,
                },
                Price5 = new ArticlePrice()
                {
                    Value = 0,
                    PromotionValue = 0,
                    UsePromotion = false,
                }
            };

            var articleResult = DependencyInjection.Mediator.Send(addArticleCommand).Result;

            if (articleResult.IsError)
            {
                ErrorHandlingService.HandleApiError(articleResult);
                return;
            }

            ArticlesService.RefreshArticlesCache();
            var documentDetail = ArticlesService.GetArticleViewModel(articleResult.Value);
            SelectArticle(documentDetail);
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
                TxtSerialNumber.Clear();
                TxtQuantity.Component.Sensitive = true;
            }
        }

        private void BtnSelectFamily_Clicked(object sender, EventArgs e)
        {
            var page = new ArticleFamiliesPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<ArticleFamily>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtFamily.SelectedEntity= page.SelectedEntity;
                TxtFamily.Text = page.SelectedEntity.Designation;
            }
        }

        private void BtnSelectSubFamily_Clicked(object sender, EventArgs e)
        {
            if(TxtFamily.SelectedEntity == null)
            {
                return;
            }
            ArticleSubfamiliesPage.FamilyId = (TxtFamily.SelectedEntity as ArticleFamily).Id;
            var page = new ArticleSubfamiliesPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<ArticleSubfamily>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtSubFamily.SelectedEntity = page.SelectedEntity;
                TxtSubFamily.Text = page.SelectedEntity.Designation;
            }
        }

        private void ArticleAutocompleteLine_Selected(object article)
        {
            SelectArticle(article as ArticleViewModel);
            TxtSerialNumber.Clear();
            TxtQuantity.Component.Sensitive = true;
        }

        private void TxtSerialNumber_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtSerialNumber.Text))
            {
                Clear();
            }
        }

        private void SerialNumberAutocompleteLine_Selected(object article)
        {
            SelectArticle(article as ArticleViewModel);
            TxtQuantity.Text = "1";
            TxtQuantity.Component.Sensitive = false;
        }


    }
}
