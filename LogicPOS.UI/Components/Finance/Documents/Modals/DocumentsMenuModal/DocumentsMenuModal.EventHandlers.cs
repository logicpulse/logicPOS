using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Modals
{
    internal partial class DocumentsMenuModal
    {
        private void BtnAddStock_Clicked(object sender, EventArgs e)
        {
            var addStockModal = new AddStockMovementModal(WindowSettings.Source);
            addStockModal.Run();
            addStockModal.Destroy();
        }

        private void BtnWorkSessionPeriods_Clicked(object sender, EventArgs e)
        {

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
            var modal = new DocumentsModal(WindowSettings.Source);
            modal.Run();
            modal.Destroy();
        }
    }
}
