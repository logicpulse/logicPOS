using Gtk;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class VatRateModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtDesignation = TextBox.CreateDesignationField();
        private TextBox _txtValue = TextBox.Simple("global_vat_rate", true, true, RegularExpressions.DecimalNumber);
        private TextBox _txtTaxType = TextBox.Simple("global_vat_rate_tax_type", true, true, @"^[a-zA-Z]+$");
        private TextBox _txtTaxCode = TextBox.Simple("global_vat_rate_tax_code", true, true, @"^(RED|NOR|ISE|OUT|([0-9.])*|NA|NS)$");
        private TextBox _txtCountryRegionCode = TextBox.Simple("global_vat_rate_tax_country_region", true, true, RegularExpressions.AlfaCountryCode2);
        private TextBox _txtDescription = TextBox.Simple("global_vat_rate_description", true);
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
     
    }
}
