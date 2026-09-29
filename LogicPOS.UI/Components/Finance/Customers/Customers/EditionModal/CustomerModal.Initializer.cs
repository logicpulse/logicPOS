using Gtk;
using LogicPOS.Api.Entities;
using CardMode = LogicPOS.Api.Features.Finance.Customers.Customers.Common.CardMode;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Services;
using LogicPOS.Globalization;
using LogicPOS.Utility;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerModal
    {

        protected override void Initialize()
        {
            InitializeCountriesComboBox();
            InitializePriceTypesComboBox();
            InitializeCustomerTypesComboBox();
            _txtCardCredit.Text = "0";
            _txtCardCredit.Component.Sensitive = false;
            InitializeCardModeComboBox();

            if (_modalMode == EntityEditionModalMode.Insert && SystemInformationService.SystemInformation.IsAngola)
            {
                AddFillCustomerDataBtn();
            }
        }

        private void InitializeCustomerTypesComboBox()
        {
            var customerTypes = GetCustomerTypes();
            var labelText = LocalizedString.Instance["global_customer_types"];
            var currentCustomerType = _entity != null ?
                new CustomerType { Id = _entity.CustomerType.Id, Designation = _entity.CustomerType.Designation } :
                customerTypes?.FirstOrDefault();

            _comboCustomerTypes = new EntityComboBox<CustomerType>(labelText,
                                                             customerTypes,
                                                             currentCustomerType,
                                                             true);
        }

        private void InitializePriceTypesComboBox()
        {
            var priceTypes = GetPriceTypes();
            var labelText = LocalizedString.Instance["global_price_type"];
            var currentPriceType = _entity != null ?
                new PriceType { Id = _entity.PriceType.Id, Designation = _entity.PriceType.Designation } :
                priceTypes?.FirstOrDefault();

            _comboPriceTypes = new EntityComboBox<PriceType>(labelText,
                                                             priceTypes,
                                                             currentPriceType,
                                                             true);
        }

        private void InitializeCardModeComboBox()
        {
            _lblCardMode = new Label(LocalizedString.Instance["global_card_mode"]);
            _lblCardMode.SetAlignment(0, 0);
            _comboCardMode = new ComboBox(new[]
            {
                LocalizedString.Instance["global_debit"],
                LocalizedString.Instance["global_credit"]
            });
            _comboCardMode.Active = (int)CardMode.Debit;

            _cardModeComponent = new VBox(false, 2);
            _cardModeComponent.PackStart(_lblCardMode, false, false, 0);
            _cardModeComponent.PackStart(_comboCardMode, false, false, 0);
        }

        private void InitializeCountriesComboBox()
        {
            var labelText = LocalizedString.Instance["global_country"];
            var currentCountry = _entity != null ?
                new Country { Id = _entity.Country.Id, Designation = _entity.Country.Designation } :
                CountriesService.Default;

            _comboCountries = new EntityComboBox<Country>(labelText,
                                                             CountriesService.Countries,
                                                             currentCountry,
                                                             true);

            _comboCountries.ComboBox.Changed += ComboBoxCountries_Changed;
        }

    }
}
