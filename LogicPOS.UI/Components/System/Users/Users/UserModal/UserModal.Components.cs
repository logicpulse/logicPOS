using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UserModal
    {
        private TextBox _txtOrder = TextBox.CreateOrderField();
        private TextBox _txtCode = TextBox.CreateCodeField();
        private TextBox _txtName = TextBox.Simple("global_name", true);
        private EntityComboBox<UserProfile> _comboProfiles;
        private EntityComboBox<CommissionGroup> _comboCommissionGroups;
        private TextBox _txtContractDate = TextBox.Simple("global_contract_date");

        private TextBox _txtAddress = TextBox.Simple("global_address");
        private TextBox _txtLocality = TextBox.Simple("global_locality");
        private TextBox _txtCity = TextBox.Simple("global_city");
        private TextBox _txtPostalCode = TextBox.Simple("global_postal_code");
        private TextBox _txtPhone = TextBox.Simple("global_phone");
        private TextBox _txtMobile = TextBox.Simple("global_mobile_phone");
        private TextBox _txtEmail = TextBox.Simple("global_email_separator");

        private TextBox _txtFiscalNumber = TextBox.Simple("global_fiscal_number");
        private TextBox _txtLanguage = TextBox.Simple("global_language");
        private TextBox _txtAssignedSeating = TextBox.Simple("global_assigned_seating");
        private TextBox _txtAccessCardNumber = TextBox.Simple("global_access_card_number");
        private TextBox _txtConsumptionBase = TextBox.Simple("global_base_consumption");
        private TextBox _txtBaseOffers = TextBox.Simple("global_base_offers");
        private TextBox _txtPVPOffers = TextBox.Simple("global_price_offers");
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
    }
}
