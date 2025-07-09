using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Drawing;
using System.Linq;
using Item = LogicPOS.UI.Components.Documents.CreateDocument.Item;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddArticleModal : Modal
    {
        private EntityEditionModalMode _mode;
        public Item Item { get; }
        private decimal _vatRateValue;
        public AddArticleModal(Window parent,
                               EntityEditionModalMode mode,
                               Item item = null) : base(parent,
                                                     GeneralUtils.GetResourceByName("global_insert_articles"),
                                                     new Size(900, 360),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_finance_article.png")
        {
            _mode = mode;
            Item = item;

            HandleModalMode();
        }

        private void HandleModalMode()
        {
            if (_mode != EntityEditionModalMode.Insert)
            {
                ShowItemData(Item);
            }

            if (_mode == EntityEditionModalMode.View)
            {
                DisableFields();
            }

            if (_mode == EntityEditionModalMode.Update)
            {
                TxtArticle.Component.Sensitive = false;
            }
        }

        private void DisableFields()
        {
            TxtArticle.Component.Sensitive = false;
            TxtPrice.Component.Sensitive = false;
            TxtQuantity.Component.Sensitive = false;
            TxtDiscount.Component.Sensitive = false;
            TxtTax.Component.Sensitive = false;
            TxtVatExemptionReason.Component.Sensitive = false;
            TxtNotes.Component.Sensitive = false;
        }

        private void ShowItemData(Item item)
        {
            TxtArticle.SelectedEntity = item.Article;
            TxtArticle.Text = item.Designation;
            TxtPrice.Text = item.UnitPrice.ToString("F2");
            TxtQuantity.Text = item.Quantity.ToString("F2");
            TxtDiscount.Text = item.Discount.ToString("F2");
            TxtVatExemptionReason.SelectedEntity = item.VatExemptionReason;
            TxtVatExemptionReason.Text = item.VatExemptionReason?.Designation ?? item.ExemptionReason;
            TxtTax.SelectedEntity = item.VatRate;
            TxtTax.Text = item.VatRate?.Designation ?? item.VatDesignation;
            _vatRateValue = item.Vat;
            TxtNotes.Text = item.Notes;
            UpdateTotals();
            UpdateValidatableFields();
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
            AddEventHandlers();
        }

        private void AddEventHandlers()
        {
            BtnClear.Clicked += BtnClear_Clicked;
            BtnOk.Clicked += BtnOk_Clicked;
        }

        private void UpdateValidatableFields()
        {
            if (_vatRateValue != 0)
            {
                TxtVatExemptionReason.Clear();
                ValidatableFields.Remove(TxtVatExemptionReason);
                TxtVatExemptionReason.Component.Sensitive = false;
            }
            else
            {
                ValidatableFields.Add(TxtVatExemptionReason);
                TxtVatExemptionReason.IsRequired = true;
                TxtVatExemptionReason.Component.Sensitive = true;
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
            _vatRateValue = article.VatDirectSelling.Value;
            TxtNotes.Text = article.Notes;
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
                VatRate = TxtTax.SelectedEntity as VatRate,
                VatDesignation = TxtTax.Text,
                Vat = _vatRateValue,
                VatRateId = (TxtTax.SelectedEntity as VatRate).Id,
                VatExemptionReason = TxtVatExemptionReason.SelectedEntity as VatExemptionReason,
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

            ValidationUtilities.ShowValidationErrors(ValidatableFields);

            Run();
        }

        protected bool AllFieldsAreValid()
        {
            return ValidatableFields.All(txt => txt.IsValid());
        }

        private void UpdateTotals()
        {
            if (decimal.TryParse(TxtPrice.Text, out decimal price) &&
                decimal.TryParse(TxtQuantity.Text, out decimal quantity) &&
                decimal.TryParse(TxtDiscount.Text, out decimal discount))
            {
                var subTotal = price * quantity;
                var discountPrice = subTotal * discount / 100;
                var totalNet = subTotal - discountPrice;
                TxtTotal.Text = totalNet.ToString("0.00");
                var vatRatePrice = totalNet * _vatRateValue / 100;
                var totalWithTax = totalNet + vatRatePrice;
                TxtTotalWithTax.Text = totalWithTax.ToString("0.00");

                return;
            }

            TxtTotal.Clear();
            TxtTotalWithTax.Clear();
        }
    }
}
