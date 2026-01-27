using Gtk;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Finance.Agt;
using LogicPOS.UI.Components.Terminals;
using Serilog;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class ReceiptsModal
    {
        private void AddButtonsEventHandlers()
        {
            BtnOpenDocument.Clicked += BtnOpenDocument_Clicked;
            BtnPrintDocumentAs.Clicked += BtnPrintDocumentAs_Clicked;
            BtnCancelDocument.Clicked += BtnCancelReceipt_Clicked;
            BtnPrintDocument.Clicked += BtnPrintDocument_Clicked;
            BtnSendDocumentEmail.Clicked += BtnSendDocumentEmail_Clicked;
            BtnSendDocumentToAgt.Clicked += BtnSendDocumentToAgt_Clicked;
            BtnUpdateAgtValidationStatus.Clicked += BtnUpdateAgtValidationStatus_Clicked;
            BtnViewAgtDocument.Clicked += BtnViewAgtDocument_Clicked;
        }

        private void BtnViewAgtDocument_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null || Page.SelectedEntity.AgtInfo?.Number == null)
            {
                return;
            }

            var agtDocument = AgtService.GetAgtDocument(Page.SelectedEntity.Id);

            AgtDocumentInfoModal.Show(agtDocument, this);
        }

        private void BtnUpdateAgtValidationStatus_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var result = AgtService.UpdateDocumentValidationStatus(Page.SelectedEntity.Id);

            if (result == false)
            {
                CustomAlerts.Error(this)
                .WithMessage($"Não foi possível atualizar o estado de validação do documento {Page.SelectedEntity.RefNo}.")
                .ShowAlert();
                return;
            }

            CustomAlerts.Information(this)
               .WithMessage($"O estado de validação do documento {Page.SelectedEntity.RefNo} foi atualizado.")
               .ShowAlert();
            Page.Refresh();
        }

        private void BtnSendDocumentToAgt_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var advance = CustomAlerts.Question(this)
                .WithMessage($"Tem a certeza pretende enviar o documento {Page.SelectedEntity.RefNo} para a AGT? Esta acção não pode ser revertida.")
                .ShowAlert();

            if (advance != ResponseType.Yes)
            {
                return;
            }

            var result = AgtService.RegisterDocument(Page.SelectedEntity.Id);

            if (result == false)
            {
                CustomAlerts.Error(this)
                .WithMessage($"Não foi possível enviar o documento {Page.SelectedEntity.RefNo} para a AGT.")
                .ShowAlert();
                return;
            }

            CustomAlerts.Information(this)
               .WithMessage($"O documento {Page.SelectedEntity.RefNo} foi enviado à AGT com sucesso.")
               .ShowAlert();

            Page.Refresh();
        }

        private void BtnPrintDocumentAs_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var modal = new RePrintDocumentModal(this, Page.SelectedEntity.RefNo);
            ResponseType reponse = (ResponseType)modal.Run();
            var copyNumber = modal.Copies;
            var isSecondCopy = modal.SecondPrint;
            modal.Destroy();

            if (reponse != ResponseType.Ok)
            {
                return;
            }
            var copy= copyNumber.Select(c => (uint)c);

            if (Page.SelectedEntity != null)
            {
                var tempFile = DocumentPdfUtils.GetReceiptPdfFileLocation(Page.SelectedEntity.Id, 1, isSecondCopy);

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
            var copies = modal.Copies;
            var isSecondCopy = modal.SecondPrint;
            modal.Destroy();

            if (reponse != ResponseType.Ok)
            {
                return;
            }

            var tempFile = DocumentPdfUtils.GetReceiptPdfFileLocation(Page.SelectedEntity.Id, (uint)copies.First(), isSecondCopy);

            if (tempFile == null)
            {
                return;
            }

            try
            {
                PdfPrinter.Print(tempFile.Value.Path, printer.Designation);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error printing document {DocumentId}", Page.SelectedEntity.Id);
                CustomAlerts.Error(this)
                            .WithMessage($"Ocorreu um erro ao tentar imprimir o documento. {ex.Message}")
                            .ShowAlert();
            }
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

            var modal = new SendDocumentByEmailModal(Page.SelectedReceipts.Select(d =>( d.Id,d.RefNo)),
                                                     Page.SelectedEntity.CustomerFiscalNumber,
                                                     true,
                                                     this);

            var response = (ResponseType)modal.Run();
            modal.Destroy();
        }
    }
}
