using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.DeleteDraft;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetDetails;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetDocumentPreviewData;
using LogicPOS.Api.Features.Finance.Documents.Documents.IssueDocument;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.AddDocumentPrint;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetPrintingModel;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static Guid? IssueDocument(IssueDocumentCommand command)
        {
            var document = DependencyInjection.Mediator.Send(command).Result;
            if (document.IsError != false)
            {
                ErrorHandlingService.HandleApiError(document);
                return null;
            }

            return document.Value;
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

        public static InvoicePrintingData? IssueDocumentForPrinting(IssueDocumentCommand command)
        {
            var documentId = IssueDocument(command);
            if (documentId == null)
            {
                return null;
            }

            var document = GetPrintingModel(documentId.Value);

            if (document == null)
            {
                return null;
            }

            return new InvoicePrintingData
            {
                DocumentId = documentId.Value,
                Document = document,
                CompanyInformations = CompanyDetailsService.CompanyInformation
            };
        }

        public static void RegisterPrint(Guid? documentId, IEnumerable<int> copies, bool secondPrint, string reason = null)
        {
            var command = new AddDocumentPrintCommand(documentId, string.Join(",", copies), secondPrint, reason);
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
