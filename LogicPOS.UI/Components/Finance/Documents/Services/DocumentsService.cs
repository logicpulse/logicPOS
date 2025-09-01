using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.Api.Features.Documents.GetDocumentById;
using LogicPOS.Api.Features.Documents.GetDocuments;
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
        private static readonly ISender _mediator = DependencyInjection.Mediator;
      
        public static List<Document> GetAllDocuments()
        {
            var documents = _mediator.Send(new GetDocumentsQuery() { PageSize=1000000}).Result;

            if (documents.IsError != false)
            {
                ErrorHandlingService.HandleApiError(documents);
                return null;
            }

            return documents.Value.Items.ToList();
        }

        public static Document GetDocument(Guid id)
        {
            var document = _mediator.Send(new GetDocumentByIdQuery(id)).Result;
            if (document.IsError != false)
            {
                ErrorHandlingService.HandleApiError(document);
                return null;
            }
            return document.Value;
        }
        
        public static Guid? IssueDocument(AddDocumentCommand command)
        {
            var document = _mediator.Send(command).Result;
            if (document.IsError != false)
            {
                ErrorHandlingService.HandleApiError(document);
                return null;
            }

            return document.Value;
        }

        public static InvoicePrintingData? IssueDocumentForPrinting(AddDocumentCommand command)
        {
            var documentId = _mediator.Send(command).Result;
            if (documentId.IsError != false)
            {
                ErrorHandlingService.HandleApiError(documentId);
                return null;
            }

            var document = GetDocument(documentId.Value);

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
