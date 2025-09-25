using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Finance.Currencies;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.InputFields;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class EditCompanyDetailsModal
    {
        private void InitializeTextBoxes()
        {
            InitializeTxtCountry();
            InitializeTxtCurrency();
            InitializeTxtCompany();
            InitializeTxtBusiness();
            InitializeTxtAddress();
            InitializeTxtCity();
            InitializeTxtZipCode();
            InitializeTxtFiscalNumber();
            InitializeTxtStockCapital();
            InitializeTxtTaxEntity();
            InitializeTxtPhone();
            InitializeTxtMobile();
            InitializeTxtFax();
            InitializeTxtEmail();
            InitializeTxtWebsite();
        }

        private void InitializeTxtWebsite()
        {
            TxtWebsite = new TextBox(this,
                                       GeneralUtils.GetResourceByName("prefparam_company_website"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true,
                                       includeClearButton: false);
        }

        private void InitializeTxtEmail()
        {
            TxtEmail = new TextBox(this,
                                       GeneralUtils.GetResourceByName("prefparam_company_email"),
                                       isRequired: false,
                                       isValidatable: true,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true,
                                       regex: RegularExpressions.Email,
                                       includeClearButton: false);
        }

        private void InitializeTxtFax()
        {
            TxtFax = new TextBox(this,
                                       GeneralUtils.GetResourceByName("prefparam_company_fax"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true, includeClearButton: false);
        }

        private void InitializeTxtMobile()
        {
            TxtMobile = new TextBox(this,
                                       GeneralUtils.GetResourceByName("prefparam_company_mobilephone"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true, includeClearButton: false);
        }

        private void InitializeTxtPhone()
        {
            TxtPhone = new TextBox(this,
                                       GeneralUtils.GetResourceByName("prefparam_company_telephone"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true, includeClearButton: false);
        }

        private void InitializeTxtTaxEntity()
        {
            TxtTaxEntity = new TextBox(this,
                                       GeneralUtils.GetResourceByName("prefparam_company_tax_entity"),
                                       isRequired: true,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true, 
                                       includeClearButton: false);

            TxtTaxEntity.Entry.IsEditable = false;
            TxtTaxEntity.Entry.Text = "Global";
        }

        private void InitializeTxtStockCapital()
        {
            TxtStockCapital = new TextBox(this,
                                          GeneralUtils.GetResourceByName("prefparam_company_stock_capital"),
                                          isRequired: true,
                                          isValidatable: true,
                                          includeSelectButton: false,
                                          includeKeyBoardButton: true,
                                          regex: RegularExpressions.DecimalNumber,
                                          includeClearButton: false);
        }

        private void InitializeTxtFiscalNumber()
        {
            TxtFiscalNumber = new TextBox(this,
                                             GeneralUtils.GetResourceByName("prefparam_company_fiscalnumber"),
                                             isRequired: true,
                                             isValidatable: true,
                                             includeSelectButton: false,
                                             includeKeyBoardButton: true,
                                             regex: RegularExpressions.FiscalNumber, includeClearButton: false);
        }

        private void InitializeTxtBusiness()
        {
            TxtBusiness = new TextBox(this,
                                       GeneralUtils.GetResourceByName("prefparam_company_business_name"),
                                       isRequired: false,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true, includeClearButton: false);
        }

        private void InitializeTxtCompany()
        {
            TxtCompany = new TextBox(this,
                                       GeneralUtils.GetResourceByName("prefparam_company_name"),
                                       isRequired: true,
                                       isValidatable: false,
                                       includeSelectButton: false,
                                       includeKeyBoardButton: true,
                                       includeClearButton: false);
        }

        private void InitializeTxtCountry()
        {
            TxtCountry = new TextBox(this,
                                         GeneralUtils.GetResourceByName("global_country"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: true,
                                         includeKeyBoardButton: false,
                                         includeClearButton: false);

            TxtCountry.Entry.IsEditable = false;
            TxtCountry.SelectEntityClicked += BtnSelectCountry_Clicked;
            TxtCountry.Text = CountriesService.Default.Designation;
            TxtCountry.SelectedEntity = CountriesService.Default;
        }

        private void InitializeTxtCurrency()
        {
            TxtCurrency = new TextBox(this,
                                          GeneralUtils.GetResourceByName("global_currency"),
                                          isRequired: true,
                                          isValidatable: false,
                                          includeSelectButton: true,
                                          includeKeyBoardButton: false, includeClearButton: false);

            TxtCurrency.Entry.IsEditable = false;

            TxtCurrency.Text = CurrenciesService.Default.Designation;
            TxtCurrency.SelectedEntity = CurrenciesService.Default;

            TxtCurrency.SelectEntityClicked += BtnSelectCurrency_Clicked;
        }

        private void InitializeTxtCity()
        {
            TxtCity = new TextBox(this,
                                      GeneralUtils.GetResourceByName("prefparam_company_city"),
                                      isRequired: true,
                                      isValidatable: false,
                                      includeSelectButton: false,
                                      includeKeyBoardButton: true, includeClearButton: false);
        }

        private void InitializeTxtZipCode()
        {
            TxtZipCode = new TextBox(this,
                                         GeneralUtils.GetResourceByName("prefparam_company_postalcode"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true, includeClearButton: false);
        }

        private void InitializeTxtAddress()
        {
            TxtAddress = new TextBox(this,
                                         GeneralUtils.GetResourceByName("prefparam_company_address"),
                                         isRequired: true,
                                         isValidatable: false,
                                         includeSelectButton: false,
                                         includeKeyBoardButton: true, includeClearButton: false);
        }


    }
}
