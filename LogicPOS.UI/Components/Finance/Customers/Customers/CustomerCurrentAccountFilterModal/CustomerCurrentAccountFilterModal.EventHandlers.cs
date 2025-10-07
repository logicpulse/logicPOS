using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.PDFViewer;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class CustomerCurrentAccountFilterModal
    {
        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            Validate();

            if (AllFieldsAreValid() == false)
            {
                return;
            }

            var result = _mediator.Send(CreateQuery()).Result;

            if (result.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(this, result.FirstError);
                Run();
            }

            LogicPOSPDFViewer.ShowPDF(result.Value.Path, result.Value.Name);
        }
        private void TxtEndDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();
            dateTimePicker.Destroy();

            if (response == ResponseType.Ok)
            {
                TxtEndDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }
        }

        private void BtnSelectCustomer_Clicked(object sender, EventArgs e)
        {
            var page = new CustomersPage(null, CustomersPage.CustomerSelectionOptions);
            var selectModal = new EntitySelectionModal<Customer>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtCustomer.Text = page.SelectedEntity.Name;
                TxtCustomer.SelectedEntity = page.SelectedEntity;
            }
        }
        private void TxtStartDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();
            dateTimePicker.Destroy();

            if (response == ResponseType.Ok)
            {
                TxtStartDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }
        }
    }
}
