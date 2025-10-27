using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Finance.Customers;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class DocumentShipToTab
    {

        private void TxtDeliveryDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(SourceWindow);
            ResponseType response = (ResponseType)dateTimePicker.Run();
            dateTimePicker.Destroy();

            if (response == ResponseType.Ok)
            {
                
                if (dateTimePicker.Calendar.Date == DateTime.Now.Date)
                {
                    TxtDeliveryDate.Text = DateTime.Now.ToLocalTime().ToString();
                    return;
                }
                TxtDeliveryDate.Text = dateTimePicker.Calendar.Date.ToLocalTime().ToString();
            }
        }


        private void TxtCountry_SelectEntityClicked(object sender, EventArgs e)
        {
            var page = new CountriesPage(null, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<Country>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCountry.Text = page.SelectedEntity.Designation;
                TxtCountry.SelectedEntity = page.SelectedEntity;
            }
        }

        private void TxtCoutry_Changed(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TxtCountry.Text))
            {
                TxtCountry.Clear();
            }
        }

        public void GetCustomerAddress(Guid customerId)
        {
            var customer=CustomersService.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer != null)
            {
                TxtAddress.Text = customer.Address;
                TxtCity.Text = customer.City;
                TxtCountry.Text = customer.Country.Designation;
                TxtCountry.SelectedEntity = customer.Country;
                TxtRegion.Text = customer.Locality;
                TxtZipCode.Text = customer.ZipCode;
            }
        }

    }
}
