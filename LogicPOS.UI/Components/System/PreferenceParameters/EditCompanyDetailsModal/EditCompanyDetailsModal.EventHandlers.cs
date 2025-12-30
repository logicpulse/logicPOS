using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Finance.Agt;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class EditCompanyDetailsModal
    {
        private void AddEventHandlers()
        {
            BtnOk.Clicked += BtnOk_Clicked;
            BtnDemo.Clicked += BtnDemo_Clicked;
            BtnAgtInfo.Clicked += BtnAgtInfo_Clicked;
        }

        private void BtnAgtInfo_Clicked(object sender, EventArgs e)
        {
            if (TxtFiscalNumber.IsValid() == false || string.IsNullOrWhiteSpace(TxtFiscalNumber.Text))
            {
                CustomAlerts.Warning(this).WithMessage("Informe o NIF").ShowAlert();
                return;
            }

            var contributor = AgtService.GetAgtContributorInfo(TxtFiscalNumber.Text);

            if (contributor == null)
            {
                CustomAlerts.Warning(this).WithMessage("Não foi possível retornar os dados online do contribuinte.").ShowAlert();
                return;
            }

            TxtCompany.Text = contributor.Name ?? "";
            TxtBusiness.Text = contributor.Name ?? "";
        }

        private void BtnSelectCountry_Clicked(object sender, EventArgs e)
        {
            var page = new CountriesPage(null, PageOptions.SelectionPageOptions);
            var selectCountryModal = new EntitySelectionModal<Country>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCountryModal.Run();
            selectCountryModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCountry.Text = page.SelectedEntity.Designation;
                TxtCountry.SelectedEntity = page.SelectedEntity;
            }
        }

        private void BtnSelectCurrency_Clicked(object sender, EventArgs e)
        {
            var page = new CurrenciesPage(null, PageOptions.SelectionPageOptions);
            var selectCurrencyModal = new EntitySelectionModal<Currency>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectCurrencyModal.Run();
            selectCurrencyModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCurrency.Text = page.SelectedEntity.Designation;
                TxtCurrency.SelectedEntity = page.SelectedEntity;
            }
        }

        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (!AllFieldsAreValid())
            {
                ShowValidationErrors();
                Run();
                return;
            }

            CompanyDetailsService.UpdateCompanyDetails(CreateCommand());
        }

        private void BtnDemo_Clicked(object sender, EventArgs e)
        {
            FillWithDemoData();
            Run();
        }

    }
}
