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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocumentModal
{
    public class AddItemModal : Modal
    {
        private EntityEditionModalMode _mode;
        public Item Item { get; }
        public IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        public IconButtonWithText BtnCancel { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
        public IconButtonWithText BtnClear { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.CleanFilter);
        public HashSet<IValidatableField> ValidatableFields { get; private set; } = new HashSet<IValidatableField>();
        public PageTextBox TxtArticle { get; set; }
        public PageTextBox TxtQuantity { get; set; }
        public PageTextBox TxtPrice { get; set; }
        public PageTextBox TxtDiscount { get; set; }
        public PageTextBox TxtTotal { get; set; }
        public PageTextBox TxtTotalWithTax { get; set; }
        public PageTextBox TxtTax { get; set; }
        public PageTextBox TxtVatExemptionReason { get; set; }
        public PageTextBox TxtNotes { get; set; }
        private Entry _txtVatRateValue = new Entry { Text = 0.ToString() };

        public AddItemModal(Window parent,
                            EntityEditionModalMode mode,
                            Item item = null) : base(parent,
                                                     GeneralUtils.GetResourceByName("global_insert_articles"),
                                                     new Size(900, 360),
                                                     PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_finance_article.png")
        {
            _mode = mode;
            Item = item;

            if (_mode != EntityEditionModalMode.Insert)
            {
                ShowItemData(Item);
            }
        }

        private void ShowItemData(Item item)
        {
            TxtArticle.SelectedEntity = item.Article;
            TxtArticle.Text = item.Designation;
            TxtPrice.Text = item.UnitPrice.ToString();
            TxtQuantity.Text = item.Quantity.ToString();
            TxtDiscount.Text = item.Discount.ToString();
            TxtVatExemptionReason.SelectedEntity = item.VatExemptionReason;
            TxtVatExemptionReason.Text = item.VatExemptionReason?.Designation ?? item.ExemptionReason;
            TxtTax.SelectedEntity = item.VatRate;
            TxtTax.Text = item.VatRate?.Designation ?? item.VatDesignation;
            _txtVatRateValue.Text = item.VatRateValue.ToString();
            TxtNotes.Text = item.Notes;
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
            BtnClear.Clicked += BtnClear_Clicked;
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            Validate();

            if(AllFieldsAreValid() == false)
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
                Item.VatDesignation = TxtTax.Text;
                Item.VatRateValue = decimal.Parse(_txtVatRateValue.Text);
                Item.VatExemptionReason = TxtVatExemptionReason.SelectedEntity as VatExemptionReason;
                Item.ExemptionReason = Item.VatExemptionReason is null ? TxtVatExemptionReason.Text : Item.VatExemptionReason.Designation;
                Item.Notes = TxtNotes.Text;
            }
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            Clear();
            this.Run();
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
                                                    isRequired: true,
                                                    isValidatable: false,
                                                    includeSelectButton: true,
                                                    includeKeyBoardButton: false);

            TxtVatExemptionReason.Entry.IsEditable = false;

            TxtVatExemptionReason.SelectEntityClicked += BtnSelectVatExemptionReason_Clicked;

            ValidatableFields.Add(TxtVatExemptionReason);
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

            ValidatableFields.Add(TxtTax);
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
                _txtVatRateValue.Text = page.SelectedEntity.Value.ToString();
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

            TxtDiscount.Text = "0";

            ValidatableFields.Add(TxtDiscount);
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

            TxtPrice.Text = "0";

            ValidatableFields.Add(TxtPrice);
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

            TxtQuantity.Text = "1";

            ValidatableFields.Add(TxtQuantity);
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

            ValidatableFields.Add(TxtArticle);
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
                TxtArticle.SelectedEntity = page.SelectedEntity;
                ShowArticleData(page.SelectedEntity);
            }
        }

        private void ShowArticleData(Article article)
        {
            TxtPrice.Text = article.Price1.Value.ToString();
            TxtQuantity.Text = article.DefaultQuantity.ToString();
            TxtDiscount.Text = article.Discount.ToString();
            TxtVatExemptionReason.SelectedEntity = article.VatExemptionReason;
            TxtVatExemptionReason.Text = article?.VatExemptionReason?.Designation;
            TxtTax.SelectedEntity = article.VatDirectSelling;
            TxtTax.Text = article.VatDirectSelling?.Designation;
            TxtNotes.Text = article.Notes;
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

        private void Clear()
        {
            TxtArticle.Clear();
            TxtPrice.Clear();
            TxtQuantity.Clear();
            TxtDiscount.Clear();
            TxtTotal.Clear();
            TxtTotalWithTax.Clear();
            TxtTax.Clear();
            TxtVatExemptionReason.Clear();
            TxtNotes.Clear();
        }
        
        public Item GetItem()
        {
            return new Item
            {
                Order = (TxtArticle.SelectedEntity as Article).Order,
                Code = (TxtArticle.SelectedEntity as Article).Code,
                Designation = TxtArticle.Text,
                Article = TxtArticle.SelectedEntity as Article,
                ArticleId = (TxtArticle.SelectedEntity as Article).Id,
                UnitPrice = decimal.Parse(TxtPrice.Text),
                Quantity = decimal.Parse(TxtQuantity.Text),
                Discount = decimal.Parse(TxtDiscount.Text),
                VatRate = (TxtTax.SelectedEntity as VatRate),
                VatDesignation = TxtTax.Text,
                VatRateValue = decimal.Parse(_txtVatRateValue.Text),
                VatExemptionReason = (TxtVatExemptionReason.SelectedEntity as VatExemptionReason),
                ExemptionReason = TxtVatExemptionReason.Text,
                Notes = TxtNotes.Text
            };
        }

        protected void Validate()
        {
            if (AllFieldsAreValid())
            {
                return;
            }

            Utilities.ShowValidationErrors(ValidatableFields);

            this.Run();
        }

        protected bool AllFieldsAreValid()
        {
            return ValidatableFields.All(txt => txt.IsValid());
        }

    }
}
