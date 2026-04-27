using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Services;
using System;

namespace LogicPOS.UI.Components.Modals
{
    internal partial class DocumentsMenuModal
    {
        private void BtnAddStock_Clicked(object sender, EventArgs e)
        {
            AddSimpleStockMovementModal.ShowModal(WindowSettings.Source);
        }

        private void BtnWorkSessionPeriods_Clicked(object sender, EventArgs e)
        {
            var modal = new WorkSessionsModal(WindowSettings.Source);
            modal.Run();
            modal.Destroy();
        }

        private void BtnReceipts_Clicked(object sender, EventArgs e)
        {
            var modal = new ReceiptsModal(WindowSettings.Source);
            modal.Run();
            modal.Destroy();
        }

        private void BtnCurrentAccount_Clicked(object sender, EventArgs e)
        {
            var modal = ReportsModal.DefaultFilterModal(this);
            modal.TxtDocumentType.Component.Visible = false;
            modal.TxtTerminal.Component.Visible = false;
            modal.TxtCustomer.Component.Visible = true;
            modal.TxtCustomer.IsRequired = true;
            modal.TxtCustomer.UpdateValidationColors();

            var response = (ResponseType)modal.Run();
            if (response == ResponseType.Ok)
            {
                if (modal.TxtCustomer.SelectedEntity != null)
                {
                    ReportsService.ShowCustomerBalanceDetailsReport(modal.StartDate, modal.EndDate, (modal.TxtCustomer.SelectedEntity as ApiEntity).Id);
                }
            }
            modal.Destroy();
        }

        private void BtnDocuments_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(WindowSettings.Source, Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.Default);
            modal.Run();
            modal.Destroy();
        }

        private void BtnReceiptsEmission_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(WindowSettings.Source, Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.UnpaidInvoices);
            modal.Run();
            modal.Destroy();
        }
    }
}
