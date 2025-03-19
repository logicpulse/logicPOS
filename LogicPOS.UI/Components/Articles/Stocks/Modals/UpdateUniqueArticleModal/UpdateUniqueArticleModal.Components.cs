using LogicPOS.Api.Enums;
using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UpdateUniqueArticleModal
    {
        private SerialNumberField SerialNumberField { get; set; } = new SerialNumberField();
        private TextBox TxtArticle { get; set; } = new TextBox(null,
                                                              "Artigo Original",
                                                              isRequired: true,
                                                              includeClearButton: false,
                                                              style: TextBoxStyle.Lite);

        private TextBox TxtExchangeArticle { get; set; } = new TextBox(null,
                                                                    "Artigo para troca",
                                                                    isRequired: true,
                                                                    includeClearButton: false,
                                                                    style: TextBoxStyle.Lite);
        private TextBox TxtSaleDocument { get; set; } = new TextBox(null,
                                                                    LocalizedString.Instance["global_document_number"],
                                                                    isRequired: true,
                                                                    includeClearButton: false,
                                                                    style: TextBoxStyle.Lite);

        private TextBox TxtSaleDate { get; set; } = new TextBox(null,
                                                                LocalizedString.Instance["global_date"],
                                                                isRequired: true,
                                                                includeClearButton: false,
                                                                style: TextBoxStyle.Lite);

        private IconButtonWithText BtnExchange { get; set; } = IconButtonWithText.Create("touchButtonPrev_DialogActionArea",
                                                                                               "Trocar",
                                                                                               @"Icons/icon_pos_nav_refresh.png");

        private IconButtonWithText BtnSell { get; set; } = IconButtonWithText.Create("touchButtonPrev_DialogActionArea",
                                                                                               "Movimento de Saída",
                                                                                               @"Icons/icon_pos_toolbar_logout_user.png");

        private void InitializeComponents()
        {
            TxtSaleDocument.Entry.IsEditable = false;
            TxtSaleDate.Entry.IsEditable = false;
            TxtExchangeArticle.Entry.IsEditable = false;
            TxtArticle.Component.Sensitive = false;

            switch (_entity.WarehouseArticle.Status)
            {
                case ArticleSerialNumberStatus.Exchanged:
                case ArticleSerialNumberStatus.Sold:
                    SerialNumberField.Component.Sensitive = false;
                    TxtSaleDate.Component.Sensitive = false;
                    TxtSaleDocument.Component.Sensitive = false;
                    BtnSell.Sensitive = false;
                    break;

                default:
                    TxtExchangeArticle.Component.Sensitive = false;
                    BtnExchange.Sensitive = false;
                    break;
            }
        }

        protected override void AddValidatableFields()
        {
            SerialNumberField.TxtSerialNumber.IsRequired = true;
            ValidatableFields.Add(SerialNumberField);
        }
    }
}
