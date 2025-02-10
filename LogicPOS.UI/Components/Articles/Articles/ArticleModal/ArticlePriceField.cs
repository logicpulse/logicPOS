using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles;
using LogicPOS.UI.Components.InputFields.Validation;
using System.Text.RegularExpressions;

namespace LogicPOS.UI.Components.InputFields
{
    public class ArticlePriceField : IValidatableField
    {
        private readonly PriceType _priceType;
        private Entry _txtPrice = new Entry() { WidthRequest = 100, Text = "0" };
        private Entry _txtPromotionPrice = new Entry() { WidthRequest = 90, Text= "0" };
        private CheckButton _checkUsePromotionPrice = new CheckButton(string.Empty) { WidthRequest = 160 };

        public ArticlePrice Price => new ArticlePrice
        {
            Value = decimal.Parse(_txtPrice.Text),
            PromotionValue = decimal.Parse(_txtPromotionPrice.Text),
            UsePromotion = _checkUsePromotionPrice.Active
        };

        public HBox Component { get; private set; }

        public string FieldName => _priceType.Designation;

        public ArticlePriceField(PriceType priceType, ArticlePrice current = null)
        {
            _priceType = priceType;

            if (current != null)
            {
                _txtPrice.Text = current.Value.ToString();
                _txtPromotionPrice.Text = current.PromotionValue.ToString();
                _checkUsePromotionPrice.Active = current.UsePromotion;
            }

            AddEventHandlers();

            Component = CreateComponent();
        }

        private HBox CreateComponent()
        {
            Label label = new Label(_priceType.Designation) { WidthRequest = 100 };
            label.SetAlignment(0.0F, 0.5F);

            var hbox = new HBox(false, 5);
            hbox.PackStart(label, true, true, 0);
            hbox.PackStart(_txtPrice, false, false, 0);
            hbox.PackStart(_txtPromotionPrice, false, false, 0);
            hbox.PackStart(_checkUsePromotionPrice, false, false, 0);

            return hbox;
        }

        private void AddEventHandlers()
        {
            _txtPrice.Changed += (sender, args) =>
            {
                UpdatePriceValidationColors();
            };

            _txtPromotionPrice.Changed += (sender, args) =>
            {
                UpdatePromotionValidationColors();
            };
        }

        private void UpdatePriceValidationColors()
        {
            ValidationColors.Default.UpdateComponentBackgroundColor(_txtPrice, PriceIsValid(_txtPrice));
        }

        private void UpdatePromotionValidationColors()
        {
            ValidationColors.Default.UpdateComponentBackgroundColor(_txtPromotionPrice, PriceIsValid(_txtPromotionPrice));
        }

        private bool PriceIsValid(Entry txt)
        {
            if (string.IsNullOrEmpty(txt.Text))
            {
                return false;
            }

            return Regex.IsMatch(txt.Text, RegularExpressions.Money);
        }

        public bool IsValid()
        {
            return PriceIsValid(_txtPrice) && PriceIsValid(_txtPromotionPrice);
        }
    }
}
