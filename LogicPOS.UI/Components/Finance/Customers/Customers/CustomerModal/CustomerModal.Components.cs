using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtName = TextBox.Simple("global_name", true);
        private EntityComboBox<PriceType> _comboPriceTypes;
        private EntityComboBox<CustomerType> _comboCustomerTypes;
        private EntityComboBox<Country> _comboCountries;
        private TextBox _txtBirthDate = TextBox.Simple("global_dob", false, true, @"^\d{4}/\d{2}/\d{2}$");

        private TextBox _txtAddress = TextBox.Simple("global_address");
        private TextBox _txtLocality = TextBox.Simple("global_locality");
        private TextBox _txtCity = TextBox.Simple("global_city");
        private TextBox _txtPostalCode = TextBox.Simple("global_postal_code");
        private TextBox _txtPhone = TextBox.Simple("global_phone");
        private TextBox _txtMobile = TextBox.Simple("global_mobile_phone");
        private TextBox _txtEmail = TextBox.Simple("global_email_separator");

        private TextBox _txtDiscount = TextBox.Simple("global_discount", true, true, RegularExpressions.Money);
        private TextBox _txtFiscalNumber = TextBox.Simple("global_fiscal_number", true, true, RegularExpressions.FiscalNumber);
        private TextBox _txtCardNumber = TextBox.Simple("global_card_number");
        private TextBox _txtCardCredit = TextBox.Simple("global_card_credit_amount", true, true, RegularExpressions.DecimalNumber);
        private CheckButton _checkSupplier = new CheckButton(GeneralUtils.GetResourceByName("global_supplier"));
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
    }
}
