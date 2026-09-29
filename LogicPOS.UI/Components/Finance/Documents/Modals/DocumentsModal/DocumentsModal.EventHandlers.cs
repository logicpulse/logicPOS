using Gtk;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using LogicPOS.Printing.Services;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Finance.DocumentTypes;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Components.Users;
using LogicPOS.UI.Printing;
using LogicPOS.Utility;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using LogicPOS.Globalization;

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
            BtnRefresh.Clicked += BtnRefresh_Clicked;
        }

        private void BtnRefresh_Clicked(object sender, EventArgs e)
        {
            DocumentsCache.Clear(DependencyInjection.Services.GetRequiredService<IKeyedMemoryCache>());
            Page.Refresh();
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

            var documentType = DocumentTypesService.GetByAcronym(Page.SelectedEntity.Type);
            if (!TryResolvePrintOptions(documentType, out var copies, out var isSecondCopy, out var reason))
            {
                return;
            }

            var tempFile = DocumentPdfUtils.GetDocumentPdfFileLocation(Page.SelectedEntity.Id, copies, isSecondCopy);

            if (tempFile == null)
            {
                return;
            }

            try
            {
                if (PdfPrinter.PrintWithNativeDialog(tempFile.Value.Path) == global::System.Windows.Forms.DialogResult.OK)
                {
                    DocumentsService.RegisterPrint(Page.SelectedEntity.Id, copies, isSecondCopy, reason);
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
                if (ThermalPrintingService.DocumentWasPrintedByThermalPrinter(Page.SelectedEntity.Id))
                {
                    var message = string.Format(LocalizedString.Instance["window_dialog_cant_open_document"], Page.SelectedEntity.Number);
                    CustomAlerts.Warning(this)
                                 .WithMessage(message)
                                 .ShowAlert();
                    return;

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

            var printer = ResolveDocumentPrinter(Page.SelectedEntity.Id);

            if (printer == null)
            {
                CustomAlerts.Warning(this)
                            .WithMessage("Não foi possível encontrar a impressora configurada para o terminal.")
                            .ShowAlert();
                return;
            }

            if (!CheckPrinterCompatibility(printer))
            {
                return;
            }

            var documentType = DocumentTypesService.GetByAcronym(Page.SelectedEntity.Type);
            if (!TryResolvePrintOptions(documentType, out var copies, out var isSecondCopy, out var reason))
            {
                return;
            }

            ExecuteDocumentPrint(printer, copies, isSecondCopy, reason, documentType?.PrintOpenDrawer == true && !isSecondCopy);
        }

        private bool TryResolvePrintOptions(
            LogicPOS.Api.Features.Finance.Documents.Types.Common.DocumentType documentType,
            out List<int> copies,
            out bool isSecondCopy,
            out string reason)
        {
            copies = null;
            isSecondCopy = false;
            reason = null;

            var printCopies = GetPrintCopies(documentType);

            if (DocumentsService.WasPrinted(Page.SelectedEntity.Id))
            {
                var modal = new RePrintDocumentModal(
                    this,
                    Page.SelectedEntity.Number,
                    printCopies,
                    documentType?.PrintRequestMotive == true);
                ResponseType reponse = (ResponseType)modal.Run();
                copies = modal.Copies;
                isSecondCopy = modal.SecondPrint;
                reason = modal.Reason;
                modal.Destroy();

                return reponse == ResponseType.Ok;
            }

            copies = Enumerable.Range(1, printCopies).ToList();
            return true;
        }

        private static Api.Entities.Printer ResolveDocumentPrinter(Guid documentId)
        {
            var terminal = TerminalService.Terminal;
            if (terminal == null)
            {
                return null;
            }

            if (ThermalPrintingService.DocumentWasPrintedByThermalPrinter(documentId)
                && terminal.ThermalPrinter != null)
            {
                return terminal.ThermalPrinter;
            }

            if (DocumentsService.IsFromOrder(documentId) && terminal.ThermalPrinter != null)
            {
                return terminal.ThermalPrinter;
            }

            return terminal.Printer ?? terminal.ThermalPrinter;
        }

        private static int GetPrintCopies(LogicPOS.Api.Features.Finance.Documents.Types.Common.DocumentType documentType)
        {
            var printCopies = documentType?.PrintCopies ?? 1;
            return Math.Max(1, Math.Min(4, printCopies));
        }

        private void ExecuteDocumentPrint(
            Api.Entities.Printer printer,
            List<int> copies,
            bool isSecondCopy,
            string reason,
            bool openDrawer)
        {
            try
            {
                if (printer.Type.ThermalPrinter)
                {
                    var orderedCopies = copies.Distinct().OrderBy(c => c).ToList();
                    for (var index = 0; index < orderedCopies.Count; index++)
                    {
                        var copyNumber = orderedCopies[index];
                        var thermalPrintingData = DocumentsService.GetPrintingData(
                            Page.SelectedEntity.Id,
                            isSecondCopy,
                            copyNumber,
                            reason);

                        if (thermalPrintingData == null)
                        {
                            return;
                        }

                        var data = thermalPrintingData.Value;
                        data.OpenDrawer = openDrawer && index == 0;
                        ThermalPrintingService.PrintInvoice(data, registerPrint: false);
                    }

                    DocumentsService.RegisterPrint(
                        Page.SelectedEntity.Id,
                        orderedCopies,
                        isSecondCopy,
                        reason,
                        true);
                    return;
                }

                var tempFile = DocumentPdfUtils.GetDocumentPdfFileLocation(Page.SelectedEntity.Id, copies, isSecondCopy);

                if (tempFile != null)
                {
                    PdfPrinter.Print(tempFile.Value.Path, printer.Designation);
                }

                if (openDrawer)
                {
                    AuthenticationService.HardwareOpenDrawer();
                }

                DocumentsService.RegisterPrint(
                    Page.SelectedEntity.Id,
                    copies,
                    isSecondCopy,
                    reason,
                    false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error printing document {DocumentId}", Page.SelectedEntity.Id);
                CustomAlerts.Error(this)
                            .WithMessage($"Ocorreu um erro ao tentar imprimir o documento. {ex.Message}")
                            .ShowAlert();
            }
        }

        private bool CheckPrinterCompatibility(Api.Entities.Printer printer)
        {
            if (printer.Type == null)
            {
                CustomAlerts.Error(this)
                            .WithMessage($"Erro ao carregar as configurações da impressora {printer.Designation}. \n\n" +
                                          "Reinicie a aplicação para recarregar as configurações.")
                            .ShowAlert();
                return false;
            }

            if (!DocumentsService.WasPrinted(Page.SelectedEntity.Id))
            {
                return true;
            }

            if (ThermalPrintingService.DocumentWasPrintedByThermalPrinter(Page.SelectedEntity.Id) && printer.Type.ThermalPrinter != true)
            {
                CustomAlerts.Warning(this)
                            .WithMessage("O documento que tentou imprimir foi Criado em uma impressora Térmica.")
                            .ShowAlert();
                return false;
            }

            if (!ThermalPrintingService.DocumentWasPrintedByThermalPrinter(Page.SelectedEntity.Id) && printer.Type.ThermalPrinter == true)
            {
                CustomAlerts.Warning(this)
                            .WithMessage("O documento que tentou imprimir não foi Criado em uma impressora Térmica.")
                            .ShowAlert();
                return false;
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

            var modal = new PayInvoiceModal(this, Page.SelectedDocuments.Where(d => d.Type == "FT" || d.Type == "ND" || d.Type == "NC"));
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

            var modal = new SendDocumentByEmailModal(Page.SelectedDocuments.Select(d => (d.Id, d.Number)),
                                                     Page.SelectedEntity.Customer.FiscalNumber,
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
