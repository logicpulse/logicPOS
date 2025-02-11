
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class UserModal
    {
        public override Size ModalSize => new Size(500, 495);
        public override string ModalTitleResourceName => "window_title_edit_user";

        #region Components
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
        #endregion

        protected override void BeforeDesign()
        {
            InitializeProfilesComboBox();
            InitializeCommissionGroupsComboBox();
        }

        private void InitializeProfilesComboBox()
        {
            var priceTypes = GetProfiles();
            var labelText = GeneralUtils.GetResourceByName("global_user_profiles");
            var currentProfile = _entity != null ? _entity.Profile : null;

            _comboProfiles = new EntityComboBox<UserProfile>(labelText,
                                                             priceTypes,
                                                             currentProfile,
                                                             true);
        }

        private void InitializeCommissionGroupsComboBox()
        {
            var commissionGroups = GetCommissionGroups();
            var labelText = GeneralUtils.GetResourceByName("global_commission_group");
            var currentCommissionGroup = _entity != null ? _entity.CommissionGroup : null;

            _comboCommissionGroups = new EntityComboBox<CommissionGroup>(labelText,
                                                             commissionGroups,
                                                             currentCommissionGroup,
                                                             false);
        }

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtName.Entry);
            SensitiveFields.Add(_comboProfiles.ComboBox);
            SensitiveFields.Add(_comboCommissionGroups.ComboBox);
            SensitiveFields.Add(_txtContractDate.Entry);
            SensitiveFields.Add(_txtAddress.Entry);
            SensitiveFields.Add(_txtLocality.Entry);
            SensitiveFields.Add(_txtCity.Entry);
            SensitiveFields.Add(_txtPostalCode.Entry);
            SensitiveFields.Add(_txtPhone.Entry);
            SensitiveFields.Add(_txtMobile.Entry);
            SensitiveFields.Add(_txtEmail.Entry);
            SensitiveFields.Add(_txtFiscalNumber.Entry);
            SensitiveFields.Add(_txtLanguage.Entry);
            SensitiveFields.Add(_txtAssignedSeating.Entry);
            SensitiveFields.Add(_txtAccessCardNumber.Entry);
            SensitiveFields.Add(_txtConsumptionBase.Entry);
            SensitiveFields.Add(_txtBaseOffers.Entry);
            SensitiveFields.Add(_txtPVPOffers.Entry);
            SensitiveFields.Add(_checkDisabled);
        }

        protected override void AddValidatableFields()
        {
            ValidatableFields.Add(_txtName);
            ValidatableFields.Add(_comboProfiles);

            if (_modalMode == EntityEditionModalMode.Update)
            {
                ValidatableFields.Add(_txtOrder);
                ValidatableFields.Add(_txtCode);
            }
        }

        protected override IEnumerable<(VBox Page, string Title)> CreateTabs()
        {
            yield return (CreateDetailsTab(), GeneralUtils.GetResourceByName("global_record_main_detail"));
            yield return (CreateContactsTab(), GeneralUtils.GetResourceByName("global_contacts"));
            yield return (CreateOthersTab(), GeneralUtils.GetResourceByName("global_others"));
            yield return (CreateNotesTab(), GeneralUtils.GetResourceByName("global_notes"));
        }

        private VBox CreateDetailsTab()
        {
            var detailsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_txtOrder.Component, false, false, 0);
                detailsTab.PackStart(_txtCode.Component, false, false, 0);
            }

            detailsTab.PackStart(_txtName.Component, false, false, 0);
            detailsTab.PackStart(_comboProfiles.Component, false, false, 0);
            detailsTab.PackStart(_comboCommissionGroups.Component, false, false, 0);
            detailsTab.PackStart(_txtContractDate.Component, false, false, 0);

            if (_modalMode != EntityEditionModalMode.Insert)
            {
                detailsTab.PackStart(_checkDisabled, false, false, 0);
            }

            return detailsTab;
        }

        private VBox CreateContactsTab()
        {
            var contactsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            contactsTab.PackStart(_txtAddress.Component, false, false, 0);
            contactsTab.PackStart(_txtLocality.Component, false, false, 0);
            contactsTab.PackStart(_txtCity.Component, false, false, 0);
            contactsTab.PackStart(_txtPostalCode.Component, false, false, 0);
            contactsTab.PackStart(_txtPhone.Component, false, false, 0);
            contactsTab.PackStart(_txtMobile.Component, false, false, 0);
            contactsTab.PackStart(_txtEmail.Component, false, false, 0);

            return contactsTab;
        }

        private VBox CreateOthersTab()
        {
            var contactsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            contactsTab.PackStart(_txtFiscalNumber.Component, false, false, 0);
            contactsTab.PackStart(_txtLanguage.Component, false, false, 0);
            contactsTab.PackStart(_txtAssignedSeating.Component, false, false, 0);
            contactsTab.PackStart(_txtAccessCardNumber.Component, false, false, 0);
            contactsTab.PackStart(_txtConsumptionBase.Component, false, false, 0);
            contactsTab.PackStart(_txtBaseOffers.Component, false, false, 0);
            contactsTab.PackStart(_txtPVPOffers.Component, false, false, 0);

            return contactsTab;
        }
    }
}
