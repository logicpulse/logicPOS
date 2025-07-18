using Gtk;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddArticleModal
    {
        private List<ArticleViewModel> _articlesForCompletion;
        private List<ArticleViewModel> ArticlesForCompletion => _articlesForCompletion ?? InitializeArticlesForCompletion();

        private List<ArticleViewModel> InitializeArticlesForCompletion()
        {
            _articlesForCompletion = ArticlesService.GetAllArticles();
            return _articlesForCompletion;
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new TextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
            TxtNotes.Entry.Changed += TxtNotes_Changed; 
        }


        private void InitializeTxtVatExemptionReason()
        {
            TxtVatExemptionReason = new TextBox(WindowSettings.Source,
                                                    GeneralUtils.GetResourceByName("global_vat_exemption_reason"),
                                                    isRequired: true,
                                                    isValidatable: false,
                                                    includeSelectButton: true,
                                                    includeKeyBoardButton: false);

            TxtVatExemptionReason.Entry.IsEditable = false;

            TxtVatExemptionReason.SelectEntityClicked += BtnSelectVatExemptionReason_Clicked;

            ValidatableFields.Add(TxtVatExemptionReason);
        }

        private void InitializeTxtTax()
        {
            TxtTax = new TextBox(WindowSettings.Source,
                                     GeneralUtils.GetResourceByName("global_vat_rate"),
                                     isRequired: true,
                                     isValidatable: false,
                                     includeSelectButton: true,
                                     includeKeyBoardButton: false,
                                     regex: RegularExpressions.DecimalNumber);

            TxtTax.Entry.IsEditable = false;

            TxtTax.SelectEntityClicked += BtnSelectTax_Clicked;

            ValidatableFields.Add(TxtTax);
        }

        private void InitializeTxtTotalWithTax()
        {
            TxtTotalWithTax = new TextBox(WindowSettings.Source,
                                              GeneralUtils.GetResourceByName("global_total_per_item_vat"),
                                              isRequired: false,
                                              isValidatable: false,
                                              includeSelectButton: false,
                                              includeKeyBoardButton: false);

            TxtTotalWithTax.Entry.IsEditable = false;
        }

        private void InitializeTxtTotal()
        {
            TxtTotal = new TextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_total_article_tab"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);

            TxtTotal.Entry.IsEditable = false;
        }

        private void InitializeTxtDiscount()
        {
            TxtDiscount = new TextBox(WindowSettings.Source,
                                          GeneralUtils.GetResourceByName("global_discount"),
                                          isRequired: true,
                                          isValidatable: true,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true,
                                          regex: RegularExpressions.DecimalNumber);

            TxtDiscount.IsValidFunction = ValidationFunctions.IsValidDiscount;

            TxtDiscount.Text = "0";

            ValidatableFields.Add(TxtDiscount);

            TxtDiscount.Entry.Changed += (sender, args) => UpdateTotals();
        }

        private void InitializeTxtPrice()
        {
            TxtPrice = new TextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_price"),
                                       isRequired: true,
                                       isValidatable: true,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true,
                                       regex: RegularExpressions.Money);

            TxtPrice.Text = "0";
            ValidatableFields.Add(TxtPrice);
            TxtPrice.Entry.Changed += (sender, args) => UpdateTotals();
        }

        private void InitializeTxtQuantity()
        {
            TxtQuantity = new TextBox(WindowSettings.Source,
                                          GeneralUtils.GetResourceByName("global_quantity"),
                                          isRequired: true,
                                          isValidatable: true,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true,
                                          regex: RegularExpressions.PositiveQuantity);

            TxtQuantity.Text = "1";

            ValidatableFields.Add(TxtQuantity);
            TxtQuantity.Entry.Changed += (sender, args) => UpdateTotals();
        }

        private void InitializeTxtArticle()
        {
            TxtArticle = new TextBox(WindowSettings.Source,
                                          GeneralUtils.GetResourceByName("global_article"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtArticle.Entry.WidthRequest = 380;

            TxtArticle.SelectEntityClicked += BtnSelectArticle_Clicked;
            ValidatableFields.Add(TxtArticle);

            var articles = ArticlesForCompletion.Select(a => (a as object, a.Designation)).ToList();
            TxtArticle.WithAutoCompletion(articles);
            TxtArticle.OnCompletionSelected += TxtCode_OnCompletionSelected;
            TxtArticle.Entry.Changed += TxtArticle_Changed;
        }

        private void InitializeTxtCode()
        {
            TxtCode = new TextBox(WindowSettings.Source,
                                  GeneralUtils.GetResourceByName("global_article_code"),
                                  isRequired: true,
                                  isValidatable: false,
                                  includeSelectButton: false,
                                  includeKeyBoardButton: false);

            TxtCode.Entry.WidthRequest = 120;

            var articles = ArticlesForCompletion.Select(a => (a as object, a.Code)).ToList();
            TxtCode.WithAutoCompletion(articles);
            TxtCode.OnCompletionSelected += TxtCode_OnCompletionSelected;
            TxtCode.Entry.Changed += TxtCode_Changed;
        }

        private void InitializeTxtFamily()
        {
            TxtFamily = new TextBox(WindowSettings.Source,
                         GeneralUtils.GetResourceByName("global_article_family"),
                         isRequired: false,
                         isValidatable: false,
                         includeSelectButton: false,
                         includeKeyBoardButton: false,
                         includeClearButton: false);

            TxtFamily.Entry.IsEditable = false;
            TxtFamily.Entry.Sensitive = false;
            ValidatableFields.Add(TxtFamily);
        }

        private void InitializeTxtSubFamily()
        {
            TxtSubFamily = new TextBox(WindowSettings.Source,
                         GeneralUtils.GetResourceByName("global_article_subfamily"),
                         isRequired: false,
                         isValidatable: false,
                         includeSelectButton: false,
                         includeKeyBoardButton: false,
                         includeClearButton: false);

            TxtSubFamily.Entry.IsEditable = false;
            TxtSubFamily.Entry.Sensitive = false;
            ValidatableFields.Add(TxtSubFamily);
        }

    }
}
