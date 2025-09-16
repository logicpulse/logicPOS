using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.AddDocumentPrint;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetPrintingModel;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Services;
using System;
using System.Collections.Generic;

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

        public static Guid? IssueDocument(AddDocumentCommand command)
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

        public static InvoicePrintingData? IssueDocumentForPrinting(AddDocumentCommand command)
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
                CompanyInformations = PreferenceParametersService.CompanyInformations
            };
        }
        
        public static void RegisterPrint(Guid? documentId, IEnumerable<int> copies, bool secondPrint, string reason = null)
        {
            var command = new AddDocumentPrintCommand(documentId, string.Join(",",copies), secondPrint, reason);
            var result = DependencyInjection.Mediator.Send(command).Result;
            if (result.IsError != false)
            {
                ErrorHandlingService.HandleApiError(result);
            }
        }
    }
}
