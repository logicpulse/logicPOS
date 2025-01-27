using Gtk;
using LogicPOS.UI.Services;
using System;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReportsModal
    {
        private void BtnCompanyBillingReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
			var response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok)
            {
                ReportsService.ShowCompanyBillingReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
		}

        private void BtnCustomerBalanceSummaryReport_Clicked(object sender, EventArgs e)
        {
           CustomerCurrentAccountFilterModal.ShowModal(this);
        }

        private void BtnSalesByDocumentReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByDocumentTypeReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByDateReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByDateReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }

        private void BtnSalesByUserReport_Clicked(object sender, EventArgs e)
        {
            var modal = new ReportsFilterModal(this);
            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                ReportsService.ShowSalesByUserReport(modal.StartDate, modal.EndDate);
            }
            modal.Destroy();
        }
    }
}
