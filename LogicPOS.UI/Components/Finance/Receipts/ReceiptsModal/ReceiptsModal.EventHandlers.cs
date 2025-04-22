using Gtk;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Documents;
using System;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.Receipts.CancelReceipt;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.InputFields.Validation;
using LogicPOS.Utility;
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
                var pdfLocation = DocumentPdfUtils.GetReceiptPdfFileLocation(Page.SelectedEntity.Id, copyNumber);

                if (pdfLocation == null)
                {
                    return;
                }

                PdfPrinter.PrintWithNativeDialog(pdfLocation);
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

            var pdfLocation = DocumentPdfUtils.GetReceiptPdfFileLocation(Page.SelectedEntity.Id, copyNumber);

            if (pdfLocation == null)
            {
                return;
            }

            PdfPrinter.Print(pdfLocation, printer.Designation);
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
