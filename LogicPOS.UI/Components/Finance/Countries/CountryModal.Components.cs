using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    internal partial class CountryModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtCapital = TextBox.Simple("global_country_capital", true);
        private TextBox _txtCurrency = TextBox.Simple("global_currency", true);
        private TextBox _txtCode2 = TextBox.Simple("global_country_code2", true);
        private TextBox _txtCode3 = TextBox.Simple("global_country_code3", true);
        private TextBox _txtCurrencyCode = TextBox.Simple("global_currency_code");
        private TextBox _txtFiscalNumberRegex = TextBox.Simple("global_regex_fiscal_number");
        private TextBox _txtZipCodeRegex = TextBox.Simple("global_regex_postal_code");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
    }
}
