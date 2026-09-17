using Gtk;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class AgtDocumentsFilterModal
    {
        private void BtnOk_Clicked(object sender, EventArgs e)
        {
            if (!AllFieldsAreValid())
            {
                ShowValidationErrors();
                Run();
                return;
            }
        }

        private void BtnClear_Clicked(object sender, EventArgs e)
        {
            TxtStartDate.Clear();
            TxtEndDate.Clear();
          
        }
     
        private void TxtStartDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();

            if (response == ResponseType.Ok)
            {
                TxtStartDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }

            dateTimePicker.Destroy();
        }
        
        private void TxtEndDate_SelectEntityClicked(object sender, EventArgs e)
        {
            var dateTimePicker = new DateTimePicker(this);
            ResponseType response = (ResponseType)dateTimePicker.Run();

            if (response == ResponseType.Ok)
            {
                TxtEndDate.Text = dateTimePicker.Calendar.Date.ToString("yyyy-MM-dd");
            }

            dateTimePicker.Destroy();
        }
    }
}
