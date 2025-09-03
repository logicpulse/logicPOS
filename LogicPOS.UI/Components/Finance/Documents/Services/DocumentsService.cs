using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetPrintingModel;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Printing;
using LogicPOS.UI.Services;
using MediatR;
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

        public static DocumentPrintingModel GetPrintingModel(Guid documentId)
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
            var documentId = DependencyInjection.Mediator.Send(command).Result;
            if (documentId.IsError != false)
            {
                ErrorHandlingService.HandleApiError(documentId);
                return null;
            }

            var document = GetPrintingModel(documentId.Value);

            if (document == null)
            {
                return null;
            }

            return new InvoicePrintingData
            {
                Document = document,
                CompanyInformations = PreferenceParametersService.CompanyInformations
            };
        }
    }
}
