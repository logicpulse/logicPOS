using Gtk;
using LogicPOS.UI.Components.Pages;
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
            CustomerCurrentAccountFilterModal.ShowModal(WindowSettings.Source);
        }

        private void BtnDocuments_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(WindowSettings.Source, Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.Default);
            modal.Run();
            modal.Destroy();
        }

        private void BtnReceiptsEmission_Clicked(object sender, System.EventArgs e)
        {
            var modal = new DocumentsModal(WindowSettings.Source, Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.UnpaidInvoices);
            modal.Run();
            modal.Destroy();
        }
    }
}
