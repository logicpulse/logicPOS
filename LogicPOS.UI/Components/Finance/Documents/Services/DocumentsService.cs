using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.DeleteDraft;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetDetails;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetDocumentPreviewData;
using LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.AddDocumentPrint;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetPrintingModel;
using LogicPOS.Api.Features.Finance.Documents.Series.HasActiveSeries;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.FiscalYears;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using static LogicPOS.UI.Printing.InvoicePrinter;

namespace LogicPOS.UI.Components.Finance.Documents.Services
{
    public static class DocumentsService
    {
        public static Document GetDocument(Guid id)
        {
            var document = DependencyInjection.Mediator.Send(new GetDocumentByIdQuery(id)).Result;
            if (document.IsError != false)
            {
                ErrorHandlingService.HandleApiError(document);
                return null;
            }
            return document.Value;
        }

        public static IssueDocumentResponse? IssueDocument(IssueDocumentCommand command, Gtk.Window parent)
        {
            if (command.IsDraft == false)
            {
                if (FiscalYearsService.ConfirmProceedWhenActiveFiscalYearDiffersFromCalendarYear(parent) == false)
                {
                    return null;
                }

                if (EnsureActiveSeriesExists(command.Type, parent) == false)
                {
                    return null;
                }
            }

            var document = DependencyInjection.Mediator.Send(command).Result;
            if (document.IsError != false)
            {
                ErrorHandlingService.HandleApiError(document);
                return null;
            }

            return document.Value;
        }

        /// <summary>
        /// Verifies an active series exists for the document type (and terminal when required).
        /// Shows a warning and returns false when missing.
        /// </summary>
        public static bool EnsureActiveSeriesExists(string documentType, Gtk.Window parent = null)
        {
            if (string.IsNullOrWhiteSpace(documentType))
            {
                CustomAlerts.Warning(parent)
                    .WithSize(new Size(600, 400))
                    .WithTitleResource("global_warning")
                    .WithMessage("Tipo de documento inválido para verificar a série.")
                    .ShowAlert();
                return false;
            }

            var terminalId = TerminalService.Terminal?.Id;
            var result = DependencyInjection.Mediator
                .Send(new HasActiveDocumentSeriesQuery(documentType.Trim(), terminalId))
                .Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, source: parent);
                return false;
            }

            if (result.Value.HasActiveSeries)
            {
                return true;
            }

            CustomAlerts.Warning(parent)
                .WithSize(new Size(600, 400))
                .WithTitleResource("global_warning")
                .WithMessage(
                    $"Não existe uma série activa para o tipo de documento «{documentType.Trim().ToUpperInvariant()}».\n\n" +
                    "Crie uma série deste tipo no BackOffice (Séries) antes de emitir.")
                .ShowAlert();

            return false;
        }

        private static DocumentPrintingModel GetPrintingModel(Guid documentId)
        {
            var document = DependencyInjection.Mediator.Send(new GetDocumentPrintingModelQuery(documentId)).Result;
            if (document.IsError != false)
            {
                ErrorHandlingService.HandleApiError(document);
                return null;
            }
            return document.Value;
        }

        public static InvoicePrintingData? IssueDocumentForPrinting(IssueDocumentCommand command, Gtk.Window parent)
        {
            var issueDocumentReponse = IssueDocument(command,parent);
            if (issueDocumentReponse == null)
            {
                return null;
            }

            return GetPrintingData(issueDocumentReponse.Value.Id);
        }

        public static InvoicePrintingData? GetPrintingData(
            Guid documentId,
            bool isSecondCopy = false,
            int copyNumber = 1,
            string reason = null)
        {
            var document = GetPrintingModel(documentId);

            if (document == null)
            {
                return null;
            }

            return new InvoicePrintingData
            {
                DocumentId = documentId,
                Document = document,
                CompanyInformations = CompanyDetailsService.CompanyInformation,
                IsSecondCopy = isSecondCopy,
                CopyNumber = copyNumber > 0 ? copyNumber : 1,
                Reason = reason
            };
        }

        public static void RegisterPrint(Guid? documentId, IEnumerable<int> copies, bool secondPrint, string reason = null, bool isThermal=false)
        {
            var command = new AddDocumentPrintCommand(documentId, string.Join(",", copies), secondPrint, reason, isThermal);
            var result = DependencyInjection.Mediator.Send(command).Result;
            if (result.IsError != false)
            {
                ErrorHandlingService.HandleApiError(result);
            }
        }

        public static IEnumerable<Api.Entities.DocumentDetail> GetDocumentDetails(Guid documentId)
        {
            var document = DependencyInjection.Mediator.Send(new GetDocumentDetailsQuery(documentId)).Result;
            if (document.IsError != false)
            {
                ErrorHandlingService.HandleApiError(document);
                return Enumerable.Empty<Api.Entities.DocumentDetail>();
            }
            return document.Value;
        }

        public static void DeleteDraft(Guid draftId)
        {
            var result = DependencyInjection.Mediator.Send(new DeleteDraftCommand(draftId)).Result;
            if (result.IsError != false)
            {
                ErrorHandlingService.HandleApiError(result);
            }
        }

        public static Document GetPreviewData(GetDocumentPreviewDataQuery query)
        {
            var document = DependencyInjection.Mediator.Send(query).Result;
            if (document.IsError != false)
            {
                ErrorHandlingService.HandleApiError(document);
                return null;
            }

            return document.Value;
        }
    }
}
