using Gtk;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Terminals;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Documents
{
    public partial class DocumentsModal
    {
        private void BtnPrintDocumentAs_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var modal = new RePrintDocumentModal(this, Page.SelectedEntity);
            modal.Run();
            modal.Destroy();

            var pdfLocation = DocumentPdfUtils.GetDocumentPdfFileLocation(Page.SelectedEntity.Id);

            if (pdfLocation == null)
            {
                return;
            }

            PdfPrinter.PrintWithNativeDialog(pdfLocation);
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

                BtnPrintDocumentAs_Clicked(sender, e);
                return;
            }

            var pdfLocation = DocumentPdfUtils.GetDocumentPdfFileLocation(Page.SelectedEntity.Id);

            if (pdfLocation == null)
            {
                return;
            }

            PdfPrinter.Print(pdfLocation, printer.Designation);
        }

        private void BtnPayInvoice_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedDocuments.Count == 0)
            {
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
    }
}
