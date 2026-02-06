using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Stocks.WarehouseArticles.Common;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddArticleModal
    {
        private void Initialize()
        {
            InitializeTxtCode();
            InitializeTxtArticle();
            InitializeTxtSerialNumber();
            InitializeTxtQuantity();
            InitializeTxtPrice();
            InitializeTxtDiscount();
            InitializeTxtTotal();
            InitializeTxtTotalWithTax();
            InitializeTxtTax();
            InitializeTxtVatExemptionReason();
            InitializeTxtNotes();
            InitializeTxtFamily();
            InitializeTxtSubFamily();
            AddEventHandlers();
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new TextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
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

            TxtTotalWithTax.Entry.Sensitive = false;
            TxtTotalWithTax.Text = 0M.ToString("0.00");
        }

        private void InitializeTxtTotal()
        {
            TxtTotal = new TextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_total_article_tab"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false);

            TxtTotal.Entry.Sensitive = false;
            TxtTotal.Text = 0M.ToString("0.00");
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

            TxtDiscount.Text = 0M.ToString("0.00");

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

            TxtPrice.Text = 0M.ToString("0.00");
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

            TxtQuantity.Text = 1M.ToString("0.00");

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
            TxtArticle.WithAutoCompletion(ArticlesService.AutocompleteLines, id => ArticlesService.GetArticleViewModel(id));
            TxtArticle.OnCompletionSelected += ArticleAutocompleteLine_Selected;
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

            TxtCode.WithAutoCompletion(ArticlesService.CodeAutocompleteLines, id => ArticlesService.GetArticleViewModel(id));
            TxtCode.OnCompletionSelected += ArticleAutocompleteLine_Selected;
            TxtCode.Entry.Changed += TxtCode_Changed;
        }

        private void InitializeTxtSerialNumber()
        {
            TxtSerialNumber = new TextBox(WindowSettings.Source,
                                  GeneralUtils.GetResourceByName("global_serial_number"),
                                  isRequired: false,
                                  isValidatable: false,
                                  includeSelectButton: true,
                                  includeKeyBoardButton: false);

            TxtSerialNumber.Entry.WidthRequest = 120;

            var autoCompleteLines = ArticlesService.GetUniqueArticlesAutocompleteLines();
            TxtSerialNumber.WithAutoCompletion(autoCompleteLines, id => ArticlesService.GetArticleViewModel(id));
            TxtSerialNumber.OnCompletionSelected += SerialNumberAutocompleteLine_Selected;
            TxtSerialNumber.Entry.Changed += TxtSerialNumber_Changed;
            TxtSerialNumber.SelectEntityClicked += TxtSerialNumber_SelectEntityClicked;
        }



        private void InitializeTxtFamily()
        {
            TxtFamily = new TextBox(WindowSettings.Source,
                         GeneralUtils.GetResourceByName("global_article_family"),
                         isRequired: true,
                         isValidatable: false,
                         includeSelectButton: true,
                         includeKeyBoardButton: false,
                         includeClearButton: false);

            TxtFamily.SelectEntityClicked += BtnSelectFamily_Clicked;

            TxtFamily.Entry.IsEditable = false;
            TxtFamily.Entry.Sensitive = false;
            ValidatableFields.Add(TxtFamily);
        }

        private void InitializeTxtSubFamily()
        {
            TxtSubFamily = new TextBox(WindowSettings.Source,
                         GeneralUtils.GetResourceByName("global_article_subfamily"),
                         isRequired: true,
                         isValidatable: false,
                         includeSelectButton: true,
                         includeKeyBoardButton: false,
                         includeClearButton: false);

            TxtSubFamily.SelectEntityClicked += BtnSelectSubFamily_Clicked;

            TxtSubFamily.Entry.IsEditable = false;
            TxtSubFamily.Entry.Sensitive = false;
            ValidatableFields.Add(TxtSubFamily);
        }

    }
}
