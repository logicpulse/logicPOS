using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Errors;
using LogicPOS.UI.PDFViewer;
using LogicPOS.Utility;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class SystemAuditFilterModal
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
                ErrorHandlingService.HandleApiError(result);
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

        private void BtnSelectTerminal_Clicked(object sender, EventArgs e)
        {
            var page = new TerminalsPage(this, PageOptions.SelectionPageOptions);
            var selectModal = new EntitySelectionModal<Terminal>(page, GeneralUtils.GetResourceByName("window_title_dialog_select_record"));
            ResponseType response = (ResponseType)selectModal.Run();
            selectModal.Destroy();

            if (response == ResponseType.Ok && page.SelectedEntity != null)
            {
                TxtTerminal.Text = page.SelectedEntity.Designation;
                TxtTerminal.SelectedEntity = page.SelectedEntity;
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
