using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Components.Finance.VatExemptionReasons;
using LogicPOS.UI.Components.Finance.VatRates.Service;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Drawing;
using System.Linq;
using DocumentDetail = LogicPOS.UI.Components.Documents.CreateDocument.DocumentDetail;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddArticleModal : Modal
    {
        private readonly EntityEditionModalMode _mode;
        public DocumentDetail DocumentDetail { get; }
        private decimal _vatRateValue;
        public AddArticleModal(Window parent,
                               EntityEditionModalMode mode,
                               DocumentDetail detail = null) : base(parent,
                                                     GeneralUtils.GetResourceByName("global_insert_articles"),
                                                     new Size(900, 360),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_finance_article.png")
        {
            _mode = mode;
            DocumentDetail = detail;

            HandleModalMode();
        }

        private void HandleModalMode()
        {
            if (_mode != EntityEditionModalMode.Insert)
            {
                ShowItemData(DocumentDetail);
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
            TxtCode.Component.Sensitive = false;
            TxtArticle.Component.Sensitive = false;
            TxtPrice.Component.Sensitive = false;
            TxtQuantity.Component.Sensitive = false;
            TxtDiscount.Component.Sensitive = false;
            TxtTax.Component.Sensitive = false;
            TxtVatExemptionReason.Component.Sensitive = false;
            TxtNotes.Component.Sensitive = false;
        }

        private void ShowItemData(DocumentDetail detail)
        {
            TxtCode.Text = detail.Code;
            TxtArticle.SelectedEntity = detail.Article;
            TxtArticle.Text = detail.Designation;
            TxtPrice.Text = detail.UnitPrice.ToString("F2");
            TxtQuantity.Text = detail.Quantity.ToString("F2");
            TxtDiscount.Text = detail.Discount.ToString("F2");
            TxtVatExemptionReason.SelectedEntity = detail.VatExemptionReason;
            TxtVatExemptionReason.Text = detail.VatExemptionReason?.Designation ?? detail.ExemptionReason;
            TxtTax.SelectedEntity = detail.VatRate;
            TxtTax.Text = detail.VatRate?.Designation ?? detail.VatDesignation;
            _vatRateValue = detail.Vat;
            TxtNotes.Text = detail.Notes;
            UpdateTotals();
            UpdateValidatableFields();
        }

        private void Initialize()
        {
            InitializeTxtCode();
            InitializeTxtArticle();
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

        private void SelectArticle(ArticleViewModel article)
        {
            var vatrate = VatRatesService.GetById(article.VatRateId);
            VatExemptionReason vatExempetionReason = article.VatExemptionReasonId.HasValue ? VatExemptionReasonsService.GetById(article.VatExemptionReasonId.Value) : null;
            
            TxtArticle.Text = article.Designation;
            TxtArticle.SelectedEntity = article;
            TxtCode.Text = article.Code;
            TxtPrice.Text = article.Price1.ToString("F2");
            TxtQuantity.Text = article.DefaultQuantity > 0 ? article.DefaultQuantity.ToString("F2") : 1.ToString("F2");
            TxtDiscount.Text = article.Discount.ToString("F2");

            TxtVatExemptionReason.SelectedEntity = vatExempetionReason;
            TxtVatExemptionReason.Text = vatExempetionReason?.Designation;

            TxtTax.SelectedEntity = vatrate;
            TxtTax.Text = vatrate.Designation;
            _vatRateValue = article.VatDirectSelling.Value;

            TxtNotes.Text = article.Notes;
            TxtFamily.Text = article.Subfamily;
            TxtSubFamily.Text = article.Subfamily;

            UpdateTotals();
            UpdateValidatableFields();
        }

        private void Clear()
        {
            TxtCode.Clear();
            TxtArticle.Clear();
            TxtPrice.Text = "0";
            TxtQuantity.Text = "0";
            TxtDiscount.Text = "0";
            TxtTotal.Text = "0";
            TxtTotalWithTax.Text = "0";
            TxtTax.Clear();
            TxtVatExemptionReason.Clear();
            TxtNotes.Clear();
            TxtSubFamily.Clear();
            TxtFamily.Clear();
        }

        public DocumentDetail GetDetail()
        {
            return new DocumentDetail
            {
                Order = (TxtArticle.SelectedEntity as ArticleViewModel).Order,
                Code = (TxtArticle.SelectedEntity as ArticleViewModel).Code,
                Designation = TxtArticle.Text,
                Article = TxtArticle.SelectedEntity as ArticleViewModel,
                ArticleId = (TxtArticle.SelectedEntity as ArticleViewModel).Id,
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

            TxtTotal.Text = 0M.ToString("0.00");
            TxtTotalWithTax.Text = 0M.ToString("0.00"); 
        }
    }
}
