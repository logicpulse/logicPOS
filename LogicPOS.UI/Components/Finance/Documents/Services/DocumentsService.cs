using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.UI.Errors;
using MediatR;
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
    }
}
