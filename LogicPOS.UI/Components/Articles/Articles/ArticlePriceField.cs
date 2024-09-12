using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Articles;

namespace LogicPOS.UI.Components.InputFields
{
    public class ArticlePriceField
    {
        private readonly PriceType _priceType;
        private Entry _txtPrice = new Entry() { WidthRequest = 100 };
        private Entry _txtPromotionPrice = new Entry() { WidthRequest = 90 };
        private CheckButton _checkUsePromotionPrice = new CheckButton(string.Empty) { WidthRequest = 160 };

        public ArticlePrice Price => new ArticlePrice
        {
            Value = decimal.Parse(_txtPrice.Text),
            PromotionValue = decimal.Parse(_txtPromotionPrice.Text),
            UsePromotion = _checkUsePromotionPrice.Active
        };

        public HBox Component { get; private set; }

        public ArticlePriceField(PriceType priceType, ArticlePrice current = null)
        {
            _priceType = priceType;

            if (current != null)
            {          
                _txtPrice.Text = current.Value.ToString();
                _txtPromotionPrice.Text = current.PromotionValue.ToString();
                _checkUsePromotionPrice.Active = current.UsePromotion;
            }

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
    }
}
