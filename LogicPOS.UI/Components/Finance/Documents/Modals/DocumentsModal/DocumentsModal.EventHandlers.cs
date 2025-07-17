using Gtk;
using LogicPOS.Api.Features.Documents.DeleteDraft;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentsModal
    {
        private void BtnPrintDocumentAs_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var modal = new RePrintDocumentModal(this, Page.SelectedEntity.Number);
            ResponseType reponse = (ResponseType)modal.Run();
            var copyNumber = modal.CopyNumber;
            modal.Destroy();

            if (reponse != ResponseType.Ok)
            {
                return;
            }

            var tempFile = DocumentPdfUtils.GetDocumentPdfFileLocation(Page.SelectedEntity.Id, copyNumber);

            if (tempFile == null)
            {
                return;
            }

            PdfPrinter.PrintWithNativeDialog(tempFile.Value.Path);
        }

        private void BtnOpenDocument_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity != null)
            {
                DocumentPdfUtils.ViewDocumentPdf(this, Page.SelectedEntity.Id);
            }
        }

        protected override void OnResponse(ResponseType response)
        {
            if (_selectionMode && response != ResponseType.Ok)
            {
                Page.SelectedEntity = null;
            }

            if (response != ResponseType.Close && _selectionMode == false)
            {
                Run();
            }

            base.OnResponse(response);
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

            var modal = new RePrintDocumentModal(this, Page.SelectedEntity.Number);
            ResponseType reponse = (ResponseType)modal.Run();
            var copyNumber = modal.CopyNumber;
            modal.Destroy();

            if (reponse != ResponseType.Ok)
            {
                return;
            }

            var tempFile = DocumentPdfUtils.GetDocumentPdfFileLocation(Page.SelectedEntity.Id, copyNumber);

            if (tempFile == null)
            {
                return;
            }

            PdfPrinter.Print(tempFile.Value.Path, printer.Designation);
        }

        private void BtnPayInvoice_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedDocuments.Count == 0)
            {
                return;
            }
            if (Page.SelectedEntity.IsDraft)
            {
                CustomAlerts.Warning(this)
                            .WithMessage($"Rascunhos não podem ser liquidados")
                            .ShowAlert();
                return;
            }
            var paidDocuments = string.Join(",", Page.SelectedDocuments.Where(x => x.Paid).Select(x => x.Number));

            if (paidDocuments != string.Empty)
            {
                CustomAlerts.Warning(this)
                            .WithMessage($"Os seguintes documentos já foram pagos: {paidDocuments}")
                            .ShowAlert();
                return;
            }


            var modal = new PayInvoiceModal(this, Page.GetSelectedDocumentsWithTotals());
            var response = (ResponseType)modal.Run();
            modal.Destroy();

            if (response == ResponseType.Ok)
            {
                Page.Refresh();
            }
        }

        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            CreateDocumentModal.ShowModal(this);
            Page.Refresh();
        }

        private void BtnCancelDocument_Clicked(object sender, EventArgs e)
        {
            var selectedDocument = Page.SelectedEntity;
            if (selectedDocument == null)
            {

                return;
            }

            if (Page.SelectedEntity.IsDraft)
            {
                var deleteResult = DependencyInjection.Mediator.Send(new DeleteDraftCommand(Page.SelectedEntity.Id)).Result;
                if (deleteResult.IsError)
                {
                    ErrorHandlingService.HandleApiError(deleteResult);
                    return ;
                } 
                 Page.Refresh();
                return;
               
            }

            if (CanCancelDocument(selectedDocument) == false)
            {
                ShowCannotCancelDocumentMessage(selectedDocument.Number);
                return;
            }

            CancelDocument(selectedDocument);
        }

        private void Page_OnChanged(object sender, EventArgs e)
        {
            UpdateModalTitle();
            UpdateNavigationButtons();
        }

        private void BtnSendDocumentEmail_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedDocuments.Count == 0)
            {
                return;
            }

            var modal = new SendDocumentByEmailModal(Page.SelectedDocuments.Select(d => d.Id),
                                                     false,
                                                     this);
            var response = (ResponseType)modal.Run();
            modal.Destroy();
        }
    }
}
