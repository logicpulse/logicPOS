
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerModal
    {
        public override Size ModalSize => new Size(400, 566);
        public override string ModalTitleResourceName => "window_title_edit_customer";

        #region Components
        private TextBox _txtOrder = TextBoxes.CreateOrderField();
        private TextBox _txtCode = TextBoxes.CreateCodeField();
        private TextBox _txtName = new TextBox("global_name", true);
        private EntityComboBox<PriceType> _comboPriceTypes;
        private EntityComboBox<CustomerType> _comboCustomerTypes;
        private EntityComboBox<Country> _comboCountries;
        private TextBox _txtBirthDate = new TextBox("global_dob",false,true, @"^\d{4}/\d{2}/\d{2}$");

        private TextBox _txtAddress = new TextBox("global_address");
        private TextBox _txtLocality = new TextBox("global_locality");
        private TextBox _txtCity = new TextBox("global_city");
        private TextBox _txtPostalCode = new TextBox("global_postal_code");
        private TextBox _txtPhone = new TextBox("global_phone");
        private TextBox _txtMobile = new TextBox("global_mobile_phone");
        private TextBox _txtEmail = new TextBox("global_email_separator");

        private TextBox _txtDiscount = new TextBox("global_discount", true,true, RegularExpressions.Money);
        private TextBox _txtFiscalNumber = new TextBox("global_fiscal_number",true,true,RegularExpressions.FiscalNumber);
        private TextBox _txtCardNumber = new TextBox("global_card_number");
        private TextBox _txtCardCredit = new TextBox("global_card_credit_amount",true,true,RegularExpressions.DecimalNumber);
        private CheckButton _checkSupplier = new CheckButton(GeneralUtils.GetResourceByName("global_supplier"));
        private CheckButton _checkDisabled = new CheckButton(GeneralUtils.GetResourceByName("global_record_disabled"));
        #endregion

        protected override void BeforeDesign()
        {
            InitializeCountriesComboBox();
            InitializePriceTypesComboBox();
            InitializeCustomerTypesComboBox();
            _txtCardCredit.Text = "0";
            _txtCardCredit.Component.Sensitive = false;
        }

        private void InitializeCustomerTypesComboBox()
        {
            var customerTypes = GetCustomerTypes();
            var labelText = GeneralUtils.GetResourceByName("global_customer_types");
            var currentCustomerType = _entity != null ? _entity.CustomerType : null;

            _comboCustomerTypes = new EntityComboBox<CustomerType>(labelText,
                                                             customerTypes,
                                                             currentCustomerType,
                                                             true);
        }

        private void InitializePriceTypesComboBox()
        {
            var priceTypes = GetPriceTypes();
            var labelText = GeneralUtils.GetResourceByName("global_price_type");
            var currentPriceType = _entity != null ? _entity.PriceType : null;

            _comboPriceTypes = new EntityComboBox<PriceType>(labelText,
                                                             priceTypes,
                                                             currentPriceType,
                                                             true);
        }

        private void InitializeCountriesComboBox()
        {
            var countries = GetCountries();
            var labelText = GeneralUtils.GetResourceByName("global_country");
            var currentCountry = _entity != null ? _entity.Country : null;

            _comboCountries = new EntityComboBox<Country>(labelText,
                                                             countries,
                                                             currentCountry,
                                                             true);
        }

        protected override void AddSensitiveFields()
        {
            SensitiveFields.Add(_txtOrder.Entry);
            SensitiveFields.Add(_txtCode.Entry);
            SensitiveFields.Add(_txtName.Entry);
            SensitiveFields.Add(_comboPriceTypes.ComboBox);
            SensitiveFields.Add(_comboCustomerTypes.ComboBox);
            SensitiveFields.Add(_txtBirthDate.Entry);
            SensitiveFields.Add(_txtAddress.Entry);
            SensitiveFields.Add(_txtLocality.Entry);
            SensitiveFields.Add(_txtCity.Entry);
            SensitiveFields.Add(_txtPostalCode.Entry);
            SensitiveFields.Add(_txtPhone.Entry);
            SensitiveFields.Add(_txtMobile.Entry);
            SensitiveFields.Add(_txtEmail.Entry);
            SensitiveFields.Add(_txtFiscalNumber.Entry);
            SensitiveFields.Add(_txtCardNumber.Entry);
            SensitiveFields.Add(_checkDisabled);
            SensitiveFields.Add(_checkSupplier);
            SensitiveFields.Add(_txtDiscount.Entry);
            SensitiveFields.Add(_comboCountries.ComboBox);
            SensitiveFields.Add(_txtNotes);
        }

        protected override void AddValidatableFields()
        {
            ValidatableFields.Add(_txtName);
            ValidatableFields.Add(_comboPriceTypes);
            ValidatableFields.Add(_txtFiscalNumber);
            ValidatableFields.Add(_txtDiscount);
            ValidatableFields.Add(_txtBirthDate);
            ValidatableFields.Add(_comboCountries);

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

            detailsTab.PackStart(_txtFiscalNumber.Component, false, false, 0);
            detailsTab.PackStart(_txtName.Component, false, false, 0);
            detailsTab.PackStart(_txtDiscount.Component, false, false, 0);
            detailsTab.PackStart(_comboPriceTypes.Component, false, false, 0);
            detailsTab.PackStart(_comboCustomerTypes.Component, false, false, 0);
            detailsTab.PackStart(_checkSupplier, false, false, 0);

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
            contactsTab.PackStart(_txtPostalCode.Component, false, false, 0);
            contactsTab.PackStart(_txtCity.Component, false, false, 0);
            contactsTab.PackStart(_comboCountries.Component, false, false, 0);
            contactsTab.PackStart(_txtPhone.Component, false, false, 0);
            contactsTab.PackStart(_txtMobile.Component, false, false, 0);
            contactsTab.PackStart(_txtEmail.Component, false, false, 0);

            return contactsTab;
        }

        private VBox CreateOthersTab()
        {
            var contactsTab = new VBox(false, _boxSpacing) { BorderWidth = (uint)_boxSpacing };

            contactsTab.PackStart(_txtCardNumber.Component, false, false, 0);
            contactsTab.PackStart(_txtCardCredit.Component, false, false,0);
            contactsTab.PackStart(_txtBirthDate.Component, false, false, 0);

            return contactsTab;
        }
    }
}
