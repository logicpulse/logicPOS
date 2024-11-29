using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Modals;
using System;

namespace LogicPOS.UI.Components.BackOffice
{
    internal partial class DashBoardPage
    {
 
        #region Documents
        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            var modal = new CreateDocumentModal(BackOfficeWindow.Instance);
            modal.Run();
            modal.Destroy();
        }

        private void BtnDocuments_Clicked(object sender, EventArgs e)
        {
            var modal = new DocumentsModal(BackOfficeWindow.Instance);
            modal.Run();
            modal.Destroy();
        }

        private void BtnPayments_Clicked(object sender, EventArgs e)
        {
            var modal = new ReceiptsModal(BackOfficeWindow.Instance);
            modal.Run();
            modal.Destroy();
        }

        private void BtnCurrentAccount_Clicked(object sender, EventArgs e)
        {
            var modal = new CustomerCurrentAccountFilterModal(BackOfficeWindow.Instance);
            modal.Run();
            modal.Destroy();
        }

        #endregion
    }
}
