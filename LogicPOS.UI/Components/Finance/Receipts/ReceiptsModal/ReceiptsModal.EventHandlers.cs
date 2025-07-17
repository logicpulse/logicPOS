using Gtk;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Components.Documents.Utilities;
using System;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Terminals;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReceiptsModal
    {
        private void BtnPrintDocumentAs_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var modal = new RePrintDocumentModal(this, Page.SelectedEntity.RefNo);
            ResponseType reponse = (ResponseType)modal.Run();
            var copyNumber = modal.CopyNumber;
            modal.Destroy();

            if (reponse != ResponseType.Ok)
            {
                return;
            }

            if (Page.SelectedEntity != null)
            {
                var tempFile = DocumentPdfUtils.GetReceiptPdfFileLocation(Page.SelectedEntity.Id, copyNumber);

                if (tempFile == null)
                {
                    return;
                }

                PdfPrinter.PrintWithNativeDialog(tempFile.Value.Path);
            }
        }

        private void BtnOpenDocument_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity != null)
            {
                DocumentPdfUtils.ViewReceiptPdf(this, Page.SelectedEntity.Id);
            }
        }

        private void BtnPrintDocument_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var printer = TerminalService.Terminal.Printer;

            if (printer == null)
            {
                CustomAlerts.Warning(this)
                            .WithMessage("Não foi possível encontrar a impressora configurada para o terminal.")
                            .ShowAlert();
                return;
            }

            var modal = new RePrintDocumentModal(this, Page.SelectedEntity.RefNo);
            ResponseType reponse = (ResponseType)modal.Run();
            var copyNumber = modal.CopyNumber;
            modal.Destroy();

            if (reponse != ResponseType.Ok)
            {
                return;
            }

            var tempFile = DocumentPdfUtils.GetReceiptPdfFileLocation(Page.SelectedEntity.Id, copyNumber);

            if (tempFile == null)
            {
                return;
            }

            PdfPrinter.Print(tempFile.Value.Path, printer.Designation);
        }
        protected override void OnResponse(ResponseType response)
        {
            if (response != ResponseType.Close)
            {
                Run();
            }

            base.OnResponse(response);
        }

        private void Page_OnChanged(object sender, EventArgs e)
        {
            UpdateModalTitle();
            UpdateNavigationButtons();
        }

        private void BtnCancelReceipt_Clicked(object sender, EventArgs e)
        {
            var receipt = Page.SelectedEntity;
            if (receipt == null)
            {
                return;
            }

            if (CanCancelReceipt(receipt) == false)
            {
                ShowCannotCancelReceiptMessage(receipt.RefNo);
                return;
            }

            CancelReceipt(receipt);
        }

        

        private void BtnSendDocumentEmail_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedReceipts.Count == 0)
            {
                return;
            }

            var modal = new SendDocumentByEmailModal(Page.SelectedReceipts.Select(d => d.Id),
                                                     true,
                                                     this);

            var response = (ResponseType)modal.Run();
            modal.Destroy();
        }
    }
}
