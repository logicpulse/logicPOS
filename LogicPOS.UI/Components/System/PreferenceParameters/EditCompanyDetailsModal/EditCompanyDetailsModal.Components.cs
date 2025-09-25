using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.InputFields;

namespace LogicPOS.UI.Components.Modals
{
    public partial class EditCompanyDetailsModal
    {
        private IconButtonWithText BtnOk { get; set; } = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
        private IconButtonWithText BtnDemo = IconButtonWithText.Create("touchButtonDataDemo_DialogActionArea", "Demo", @"Icons\Dialogs\icon_pos_demo.png");

        private TextBox TxtCountry { get; set; }
        private TextBox TxtCurrency { get; set; }
        private TextBox TxtCompany { get; set; }
        private TextBox TxtBusiness { get; set; }
        private TextBox TxtAddress { get; set; }
        private TextBox TxtCity { get; set; }
        private TextBox TxtZipCode { get; set; }
        private TextBox TxtFiscalNumber { get; set; }   
        private TextBox TxtStockCapital { get; set; }
        private TextBox TxtTaxEntity { get; set; }
        private TextBox TxtPhone { get; set; }
        private TextBox TxtMobile { get; set; }
        private TextBox TxtFax { get; set; }
        private TextBox TxtEmail { get; set; }
        private TextBox TxtWebsite { get; set; }
    }
}
