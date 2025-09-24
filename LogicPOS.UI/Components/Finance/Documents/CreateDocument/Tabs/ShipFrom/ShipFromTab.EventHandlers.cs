using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class ShipFromTab
    {
        private void TxtDeliveryDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(SourceWindow);
            ResponseType response = (ResponseType)dateTimePicker.Run();
            dateTimePicker.Destroy();

            if (response == ResponseType.Ok)
            {
                TxtDeliveryDate.Text = dateTimePicker.Calendar.Date.ToString();
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
    }
}
