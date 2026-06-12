using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles.Common;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Articles.Subfamilies;
using LogicPOS.UI.Components.Finance.Documents.CreateDocument.Tabs.Details.AddArticleModal;
using LogicPOS.UI.Components.Finance.VatExemptionReasons;
using LogicPOS.UI.Components.Finance.VatRates;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Components.POS;
using LogicPOS.UI.Extensions;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Drawing;
using System.Linq;
using DocumentDetail = LogicPOS.UI.Components.Documents.CreateDocument.DocumentDetail;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AddArticleModal : Modal
    {
        private readonly DocumentDetailModalMode _mode;
        public DocumentDetail DocumentDetail { get; }
        private decimal _vatRateValue;
        public AddArticleModal(Window parent,
                               DocumentDetailModalMode mode,
                               DocumentDetail detail = null,
                               string documentTypeAcronym = null) : base(parent,
                                                     LocalizedString.Instance["global_insert_articles"],
                                                     new Size(900, 360),
                                                     AppSettings.Paths.Images + @"Icons\Windows\icon_window_finance_article.png")
        {
            _mode = mode;
            DocumentDetail = detail;
            _documentTypeAcronym = documentTypeAcronym;

            HandleModalMode();
            ApplyTrvRestrictions();
        }

        private void HandleModalMode()
        {
            if (_mode != DocumentDetailModalMode.Insert)
            {
                ShowItemData(DocumentDetail);
            }

            if (_mode == DocumentDetailModalMode.View)
            {
                DisableFields();
            }

            if (_mode == DocumentDetailModalMode.CreditNoteUpdate)
            {
                TxtArticle.Component.Sensitive = false;
                TxtSubFamily.Component.Sensitive= false;
                TxtFamily.Component.Sensitive = false;
                TxtCode.Component.Sensitive = false;
                TxtSerialNumber.Component.Sensitive = false;
                TxtPrice.Component.Sensitive = _mode != DocumentDetailModalMode.CreditNoteUpdate;
            }
        }

        private void DisableFields()
        {
            TxtCode.Component.Sensitive = false;
            TxtArticle.Component.Sensitive = false;
            TxtPrice.Component.Sensitive = false;
            TxtQuantity.Component.Sensitive = false;
            TxtDiscount.Component.Sensitive = false;
            TxtVatRate.Component.Sensitive = false;
            TxtVatExemptionReason.Component.Sensitive = false;
            TxtNotes.Component.Sensitive = false;
            TxtSubFamily.Component.Sensitive = false;
            TxtFamily.Component.Sensitive = false;
            TxtSerialNumber.Component.Sensitive = false;
        }

        private void ShowItemData(DocumentDetail detail)
        {
            if(detail.Article == null)
            {
                detail.Article = ArticlesService.GetArticleViewModel(detail.ArticleId);
            }
            TxtCode.Text = detail.Code;
            TxtArticle.SelectedEntity = detail.Article;
            TxtArticle.Text = detail.Designation;
            TxtFamily.Text = detail.Article.Family;
            TxtSubFamily.Text = detail.Article.Subfamily;
            TxtSubFamily.SelectedEntity = ArticleSubfamiliesService.ArticleSubfamilies.FirstOrDefault(sf => sf.Designation == detail.Article.Subfamily);
            TxtPrice.Text = detail.UnitPrice.ToString("F2");
            TxtQuantity.Text = detail.Quantity.ToString("F2");
            TxtDiscount.Text = detail.Discount.ToString("F2");
            TxtVatExemptionReason.SelectedEntity = detail.VatExemptionReason;
            TxtVatExemptionReason.Text = detail.VatExemptionReason?.Designation ?? detail.ExemptionReason;
            TxtVatRate.SelectedEntity = detail.VatRate;
            TxtVatRate.Text = detail.VatRate?.Designation ?? detail.VatDesignation;
            _vatRateValue = detail.Vat;
            TxtNotes.Text = detail.Notes;
            TxtSerialNumber.Text = detail.SerialNumber;
            ApplySerialNumberQuantityRestriction();
            UpdateTotals();
            UpdateValidatableFields();
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

            TxtVatRate.SelectedEntity = vatrate;
            TxtVatRate.Text = vatrate.Designation;
            _vatRateValue = article.VatDirectSelling.Value;

            TxtNotes.Text = article.Notes;
            TxtFamily.Text = article.Family;
            TxtSubFamily.Text = article.Subfamily;
            TxtSubFamily.SelectedEntity= ArticleSubfamiliesService.ArticleSubfamilies.FirstOrDefault(sf => sf.Designation == article.Subfamily);

            TxtFamily.BtnSelect.Sensitive = false;
            TxtSubFamily.BtnSelect.Sensitive = false;

            UpdateTotals();
            UpdateValidatableFields();
        }

        private void Clear()
        {
            TxtSerialNumber.Clear();
            TxtCode.Clear();
            TxtArticle.Clear();
            TxtPrice.Text = "0";
            TxtQuantity.Text = "0";
            TxtDiscount.Text = "0";
            TxtTotal.Text = "0";
            TxtTotalWithTax.Text = "0";
            TxtVatRate.Clear();
            TxtVatExemptionReason.Clear();
            TxtNotes.Clear();
            TxtSubFamily.Clear();
            TxtFamily.Clear();
            TxtSubFamily.BtnSelect.Sensitive=true;
            TxtFamily.BtnSelect.Sensitive = true;
            TxtQuantity.Component.Sensitive = true;
        }

        public DocumentDetail GetDetail()
        {
            var selectedArticle = TxtArticle.SelectedEntity as ArticleViewModel;
            return new DocumentDetail
            {
                
                Order = selectedArticle.Order,
                Code = selectedArticle.Code,
                Designation = TxtArticle.Text,
                Article = selectedArticle,
                ArticleId = selectedArticle.Id,
                UnitPrice = selectedArticle.PriceWithVat? SaleItem.ExtractPriceWithoutVat(selectedArticle.Price1, _vatRateValue) : decimal.Parse(TxtPrice.Text),
                Quantity = decimal.Parse(TxtQuantity.Text),
                Discount = decimal.Parse(TxtDiscount.Text),
                VatRate = TxtVatRate.SelectedEntity as VatRate,
                VatDesignation = TxtVatRate.Text,
                SerialNumber = TxtSerialNumber.Text,
                Vat = _vatRateValue,
                VatRateId = (TxtVatRate.SelectedEntity as VatRate).Id,
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
            if (!IsSerialNumberQuantityValid())
            {
                CustomAlerts.Error()
                    .WithMessage("Artigos com número de série têm de ter quantidade 1.")
                    .ShowAlert();
            }

            Run();
        }

        protected bool AllFieldsAreValid()
        {
            return ValidatableFields.All(txt => txt.IsValid()) && IsSerialNumberQuantityValid();
        }

        private bool IsSerialNumberQuantityValid()
        {
            if (string.IsNullOrWhiteSpace(TxtSerialNumber.Text))
            {
                return true;
            }

            return decimal.TryParse(TxtQuantity.Text, out var quantity) && quantity == 1;
        }

        private void ApplySerialNumberQuantityRestriction()
        {
            if (string.IsNullOrWhiteSpace(TxtSerialNumber.Text))
            {
                TxtQuantity.Component.Sensitive = true;
                return;
            }

            TxtQuantity.Text = 1M.ToString("0.00");
            TxtQuantity.Component.Sensitive = false;
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
                var vatRatePrice = totalNet * _vatRateValue / 100M;
                var totalWithTax = totalNet + vatRatePrice;
                TxtTotalWithTax.Text = totalWithTax.ToString("0.00");
                
                /*
                var article = TxtArticle.SelectedEntity as ArticleViewModel;

                if ( article!=null && article.PriceWithVat && _vatRateValue>0)
                {
                    price = SaleItem.ExtractPriceWithoutVat(price, _vatRateValue);
                    discountPrice = quantity * price * discount / 100M;
                    totalNet = quantity * price - discountPrice;
                    vatRatePrice = totalNet * _vatRateValue / 100M;
                    totalWithTax = totalNet + vatRatePrice;
                    TxtPrice.Text = price.ToString("0.00");
                    TxtTotal.Text = totalNet.ToString("0.00");
                    TxtTotalWithTax.Text = totalWithTax.ToString("0.00");
                    return;
                }*/
                return;
            }

            TxtTotal.Text = 0M.ToString("0.00");
            TxtTotalWithTax.Text = 0M.ToString("0.00"); 
        }
    }
}
