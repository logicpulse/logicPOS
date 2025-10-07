using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CustomerTab
    {
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
                TxtDiscount.Text = page.SelectedEntity.Designation;
            }
        }

        private void BtnSelectCustomer_Clicked(object sender, System.EventArgs e)
        {
            var page = new CustomersPage(null, CustomersPage.CustomerSelectionOptions);
            var selectDocumentTypeModal = new EntitySelectionModal<Customer>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectDocumentTypeModal.Run();
            selectDocumentTypeModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                SelectCustomer(page.SelectedEntity);
            }
        }

        private void TxtCustomer_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtCustomer.Text))
            {
                Clear();
            }
        }

        private void TxtFiscalNumber_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtFiscalNumber.Text))
            {
                Clear();
            }
        }
    }
}
