using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Company.UpdateCompanyDetails;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class EditCompanyDetailsModal : Modal
    {
        public EditCompanyDetailsModal(Window parent) : base(parent,
            LocalizedString.Instance["window_title_edit_configurationpreferenceparameter"],
            new Size(600, 600),
            AppSettings.Paths.Images + @"Icons\Windows\icon_window_system.png")
        {
            HideCloseButton();
        }

        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnDemo.Clicked += BtnDemo_Clicked;
        }

        private void FillWithDemoData()
        {
            TxtCompany.Text = "LogicPulse";
            TxtBusiness.Text = "Technologies, Ltda";
            TxtAddress.Text = "Rua Capitão Salgueiro Maia, 7";
            TxtCity.Text = "Figueira da Foz";
            TxtZipCode.Text = "3080-000";
            TxtFiscalNumber.Text = "999999990";
            TxtStockCapital.Text = "1000";
            TxtEmail.Text = "comercial@logicpulse.com";
            TxtWebsite.Text = "www.logicpulse.com";
        }

        public bool AllFieldsAreValid() => GetValidatableFields().All(field => field.IsValid());

        private IEnumerable<IValidatableField> GetValidatableFields()
        {
            yield return TxtCountry;
            yield return TxtCurrency;
            yield return TxtCompany;
            yield return TxtBusiness;
            yield return TxtAddress;
            yield return TxtCity;
            yield return TxtZipCode;
            yield return TxtFiscalNumber;
            yield return TxtStockCapital;
            yield return TxtTaxEntity;
            yield return TxtPhone;
            yield return TxtMobile;
            yield return TxtFax;
            yield return TxtEmail;
            yield return TxtWebsite;
        }

        private void ShowValidationErrors() => ValidationUtilities.ShowValidationErrors(GetValidatableFields(), this);

        private UpdateCompanyDetailsCommand CreateCommand()
        {
            var country = TxtCountry.SelectedEntity as Country;
            return new UpdateCompanyDetailsCommand
            {
                CountryCode2 = country.Code2,
                Country = country.Designation,
                CompanyName = TxtCompany.Text,
                BusinessName = TxtBusiness.Text,
                Address = TxtAddress.Text,
                City = TxtCity.Text,
                PostalCode = TxtZipCode.Text,
                FiscalNumber = TxtFiscalNumber.Text,
                StockCapital = TxtStockCapital.Text,
                TaxEntity = TxtTaxEntity.Text,
                Phone = TxtPhone.Text,
                MobilePhone = TxtMobile.Text,
                Fax = TxtFax.Text,
                Email = TxtEmail.Text,
                Website = TxtWebsite.Text
            };
        }

        public static ResponseType ShowModal(Window parent)
        {
            var modal = new EditCompanyDetailsModal(parent);
            var response  = (ResponseType)modal.Run();
            modal.Destroy();
            return response;
        }
    }
}
