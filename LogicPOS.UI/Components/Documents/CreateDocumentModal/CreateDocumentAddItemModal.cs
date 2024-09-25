using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.Documents.CreateDocumentModal
{
    public class CreateDocumentAddItemModal : Modal
    {
        public IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        public IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        public IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);
        public PageTextBox TxtArticle { get; set; }
        public PageTextBox TxtQuantity { get; set; }
        public PageTextBox TxtPrice { get; set; }
        public PageTextBox TxtDiscount { get; set; }
        public PageTextBox TxtTotal { get; set; }
        public PageTextBox TxtTotalWithTax { get; set; }
        public PageTextBox TxtTax { get; set; }
        public PageTextBox TxtVatExemptionReason { get; set; }
        public PageTextBox TxtNotes { get; set; }

        public CreateDocumentAddItemModal(Window parent) : base(parent,
                                                                GeneralUtils.GetResourceByName("global_insert_articles"),
                                                                new Size(900, 360),
                                                                PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_finance_article.png")
        {
        }

        private void Initialize()
        {
            InitializeTxtArticle();
            InitializeTxtQuantity();
            InitializeTxtPrice();
            InitializeTxtDiscount();
            InitializeTxtTotal();
            InitializeTxtTotalWithTax();
            InitializeTxtTax();
            InitializeTxtVatExemptionReason();
            InitializeTxtNotes();
        }

        private void InitializeTxtNotes()
        {
            TxtNotes = new PageTextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_notes"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true);
        }

        private void InitializeTxtVatExemptionReason()
        {
            TxtVatExemptionReason = new PageTextBox(WindowSettings.Source,
                                                    GeneralUtils.GetResourceByName("global_vat_exemption_reason"),
                                                    isRequired: false,
                                                    isValidatable: false,
                                                    includeSelectButton: true,
                                                    includeKeyBoardButton: false);

            TxtVatExemptionReason.Entry.IsEditable = false;

            TxtVatExemptionReason.SelectEntityClicked += BtnSelectVatExemptionReason_Clicked;
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
            }
        }

        private void InitializeTxtTax()
        {
            TxtTax = new PageTextBox(WindowSettings.Source,
                                     GeneralUtils.GetResourceByName("global_vat_rate"),
                                     isRequired: true,
                                     isValidatable: false,
                                     includeSelectButton: true,
                                     includeKeyBoardButton: false,
                                     regex: RegularExpressions.DecimalNumber);

            TxtTax.Entry.IsEditable = false;

            TxtTax.SelectEntityClicked += BtnSelectTax_Clicked;
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
            }
        }

        private void InitializeTxtTotalWithTax()
        {
            TxtTotalWithTax = new PageTextBox(WindowSettings.Source,
                                              GeneralUtils.GetResourceByName("global_total_per_item_vat"),
                                              isRequired: true,
                                              isValidatable: true,
                                              includeSelectButton: false,
                                              includeKeyBoardButton: false,
                                              regex: RegularExpressions.Money);

            TxtTotalWithTax.Entry.IsEditable = false;
        }

        private void InitializeTxtTotal()
        {
            TxtTotal = new PageTextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_total_article_tab"),
                                       isRequired: true,
                                       isValidatable: true,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: false,
                                       regex: RegularExpressions.Money);

            TxtTotal.Entry.IsEditable = false;
        }

        private void InitializeTxtDiscount()
        {
            TxtDiscount = new PageTextBox(WindowSettings.Source,
                                          GeneralUtils.GetResourceByName("global_discount"),
                                          isRequired: true,
                                          isValidatable: true,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true,
                                          regex: RegularExpressions.DecimalNumber);
        }

        private void InitializeTxtPrice()
        {
            TxtPrice = new PageTextBox(WindowSettings.Source,
                                       GeneralUtils.GetResourceByName("global_price"),
                                       isRequired: true,
                                       isValidatable: true,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true,
                                       regex: RegularExpressions.Money);
        }

        private void InitializeTxtQuantity()
        {
            TxtQuantity = new PageTextBox(WindowSettings.Source,
                                          GeneralUtils.GetResourceByName("global_quantity"),
                                          isRequired: true,
                                          isValidatable: true,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true,
                                          regex: RegularExpressions.DecimalNumber);
        }

        private void InitializeTxtArticle()
        {
            TxtArticle = new PageTextBox(WindowSettings.Source,
                                          GeneralUtils.GetResourceByName("global_article"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false);

            TxtArticle.Entry.IsEditable = false;

            TxtArticle.SelectEntityClicked += BtnSelectArticle_Clicked;
        }

        private void BtnSelectArticle_Clicked(object sender, EventArgs e)
        {
            var page = new ArticlesPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<Article>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtArticle.Text = page.SelectedEntity.Designation;
                ShowArticleData(page.SelectedEntity);
            }
        }

        private void ShowArticleData(Article article)
        {

        }

        protected override ActionAreaButtons CreateActionAreaButtons()
        {
            return new ActionAreaButtons
            {
                new ActionAreaButton(BtnOk, ResponseType.Ok),
                new ActionAreaButton(BtnCancel, ResponseType.Cancel),
                new ActionAreaButton(BtnClear, ResponseType.DeleteEvent)
            };
        }

        protected override Widget CreateBody()
        {
            Initialize();

            var vbox = new VBox(false, 2);
            vbox.PackStart(TxtArticle.Component, false, false, 0);
            vbox.PackStart(PageTextBox.CreateHbox(TxtPrice,
                                                  TxtQuantity,
                                                  TxtDiscount,
                                                  TxtTotal,
                                                  TxtTotalWithTax), false, false, 0);

            vbox.PackStart(PageTextBox.CreateHbox(TxtTax, TxtVatExemptionReason), false, false, 0);

            vbox.PackStart(TxtNotes.Component, false, false, 0);

            return vbox;
        }
    }
}
