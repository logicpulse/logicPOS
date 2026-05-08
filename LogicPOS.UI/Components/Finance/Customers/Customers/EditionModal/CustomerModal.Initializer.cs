using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Services;
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

            if (_modalMode == EntityEditionModalMode.Insert && SystemInformationService.SystemInformation.IsAngola)
            {
                AddFillCustomerDataBtn();
            }
        }

        private void InitializeCustomerTypesComboBox()
        {
            var customerTypes = GetCustomerTypes();
            var labelText = GeneralUtils.GetResourceByName("global_customer_types");
            var currentCustomerType = _entity != null ?
                new CustomerType { Id = _entity.CustomerType.Id, Designation = _entity.CustomerType.Designation } :
                customerTypes.First();

            _comboCustomerTypes = new EntityComboBox<CustomerType>(labelText,
                                                             customerTypes,
                                                             currentCustomerType,
                                                             true);
        }

        private void InitializePriceTypesComboBox()
        {
            var priceTypes = GetPriceTypes();
            var labelText = GeneralUtils.GetResourceByName("global_price_type");
            var currentPriceType = _entity != null ?
                new PriceType { Id = _entity.PriceType.Id, Designation = _entity.PriceType.Designation } :
                priceTypes.First();

            _comboPriceTypes = new EntityComboBox<PriceType>(labelText,
                                                             priceTypes,
                                                             currentPriceType,
                                                             true);
        }

        private void InitializeCountriesComboBox()
        {
            var labelText = GeneralUtils.GetResourceByName("global_country");
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
