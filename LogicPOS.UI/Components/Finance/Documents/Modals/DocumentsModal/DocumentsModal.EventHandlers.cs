using Gtk;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Printing;
using Serilog;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Modals
{
    public partial class DocumentsModal
    {
        private void AddButtonsEventHandlers()
        {
            BtnOpenDocument.Clicked += BtnOpenDocument_Clicked;
            BtnPrintDocumentAs.Clicked += BtnPrintDocumentAs_Clicked;
            BtnCancelDocument.Clicked += BtnCancelDocument_Clicked;
            BtnNewDocument.Clicked += BtnNewDocument_Clicked;
            BtnPayInvoice.Clicked += BtnPayInvoice_Clicked;
            BtnPrintDocument.Clicked += BtnPrintDocument_Clicked;
            BtnSendDocumentEmail.Clicked += BtnSendDocumentEmail_Clicked;
            BtnEditDraft.Clicked += BtnEditDraft_Clicked;
            BtnDeleteDraft.Clicked += BtnDeleteDraft_Clicked;
            BtnSendDocumentToAgt.Clicked += BtnSendDocumentToAgt_Clicked;
            BtnUpdateAgtValidationStatus.Clicked += BtnUpdateAgtValidationStatus_Clicked;
            BtnViewAgtDocument.Clicked += BtnViewAgtDocument_Clicked;
            BtnSendDocumentToAt.Clicked += BtnSendDocumentToAt_Clicked;
        }

        private void BtnDeleteDraft_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            if (!Page.SelectedEntity.IsDraft)
            {
                return;
            }

            DocumentsService.DeleteDraft(Page.SelectedEntity.Id);
            Page.Refresh();
        }

        private void BtnPrintDocumentAs_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var modal = new RePrintDocumentModal(this, Page.SelectedEntity.Number);
            ResponseType reponse = (ResponseType)modal.Run();
            var copies = modal.Copies;
            bool secondPrint = modal.SecondPrint;
            string reason = modal.Reason;
            modal.Destroy();

            if (reponse != ResponseType.Ok)
            {
                return;
            }

            var printer = TerminalService.Terminal.Printer ?? TerminalService.Terminal.ThermalPrinter;
            bool canPrint = CheckPrinterCompatibility(printer);
            if (!canPrint)
            {
                return;
            }

            var tempFile = DocumentPdfUtils.GetDocumentPdfFileLocation(Page.SelectedEntity.Id, copies);

            if (tempFile == null)
            {
                return;
            }

            try
            {
                if (PdfPrinter.PrintWithNativeDialog(tempFile.Value.Path) == System.Windows.Forms.DialogResult.OK)
                {
                    DocumentsService.RegisterPrint(Page.SelectedEntity.Id, copies, secondPrint, reason);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error printing document {DocumentId}", Page.SelectedEntity.Id);
                CustomAlerts.Error(this)
                            .WithMessage("Ocorreu um erro ao tentar imprimir o documento.")
                            .ShowAlert();
            }
        }

        private void BtnOpenDocument_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity != null)
            {
                if (ThermalPrintingService.WasPrintedByThermalPrinter(Page.SelectedEntity.Id))
                {
                       CustomAlerts.Warning(this)
                                    .WithMessage("O documento que tentou imprimir foi Criado em uma impressora Térmica.")
                                    .ShowAlert();
                        return ;
                    
                }
                DocumentPdfUtils.ViewDocumentPdf(this, Page.SelectedEntity.Id);
            }
        }

        protected override void OnResponse(ResponseType response)
        {
            if (_mode == Finance.Documents.Modals.DocumentsModal.DocumentsModalMode.Selection && response != ResponseType.Ok)
            {
                Page.SelectedEntity = null;
            }

            if (response != ResponseType.Close && IsNotSelectionMode)
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

            var printer = TerminalService.Terminal.Printer ?? TerminalService.Terminal.ThermalPrinter;

            if (printer == null)
            {
                CustomAlerts.Warning(this)
                            .WithMessage("Não foi possível encontrar a impressora configurada para o terminal.")
                            .ShowAlert();
                return;
            }

            var modal = new RePrintDocumentModal(this, Page.SelectedEntity.Number);
            ResponseType reponse = (ResponseType)modal.Run();
            var copies = modal.Copies;
            bool secondPrint = modal.SecondPrint;
            string reason = modal.Reason;
            modal.Destroy();

            if (reponse != ResponseType.Ok)
            {
                return;
            }
            bool canPrint = CheckPrinterCompatibility(printer);
            if (!canPrint)
            {
                return;
            }

            var tempFile = DocumentPdfUtils.GetDocumentPdfFileLocation(Page.SelectedEntity.Id, copies);

            if (tempFile == null)
            {
                return;
            }

            try
            {
                PdfPrinter.Print(tempFile.Value.Path, printer.Designation);
                DocumentsService.RegisterPrint(Page.SelectedEntity.Id, copies, secondPrint, reason, printer.Type.ThermalPrinter);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error printing document {DocumentId}", Page.SelectedEntity.Id);
                CustomAlerts.Error(this)
                            .WithMessage("Ocorreu um erro ao tentar imprimir o documento.")
                            .ShowAlert();
            }

            
        }
        private bool CheckPrinterCompatibility(Api.Entities.Printer printer)
        {
            if (ThermalPrintingService.WasPrintedByThermalPrinter(Page.SelectedEntity.Id))
            {
                if (!printer.Type.ThermalPrinter)
                {
                    CustomAlerts.Warning(this)
                                .WithMessage("O documento que tentou imprimir foi Criado em uma impressora Térmica.")
                                .ShowAlert();
                    return false;
                }
            }
            else
            {
                if (printer.Type.ThermalPrinter)
                {
                    CustomAlerts.Warning(this)
                                .WithMessage("O documento que tentou imprimir não foi Criado em uma impressora Térmica.")
                                .ShowAlert();
                    return false;
                }
            }

            return true;
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


            var modal = new PayInvoiceModal(this, Page.SelectedDocuments);
            var response = (ResponseType)modal.Run();
            modal.Destroy();

            if (response == ResponseType.Ok)
            {
                Page.SelectedDocuments.Clear();
                Page.Refresh();
                UpdateModalTitle();
            }
        }

        private void BtnNewDocument_Clicked(object sender, EventArgs e)
        {
            var response = CreateDocumentModal.ShowModal(this);
            if (response == ResponseType.Ok)
            {
                Page.Refresh();
            }
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

        private void BtnEditDraft_Clicked(object sender, EventArgs e)
        {
            if (Page.SelectedEntity == null)
            {
                return;
            }

            var response = CreateDocumentModal.ShowModal(this, Page.SelectedEntity);

            if (response == ResponseType.Ok)
            {
                Page.Refresh();
            }
        }
    }
}
