using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Windows;
using System;

namespace LogicPOS.UI.Components.Pages
{
    internal partial class DashBoardPage
    {

        #region Documents
        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            CreateDocumentModal.ShowModal(BackOfficeWindow.Instance);
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
