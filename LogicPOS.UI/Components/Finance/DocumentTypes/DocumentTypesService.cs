using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes;
using LogicPOS.UI.Errors;
using MediatR;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.DocumentTypes
{
    public static class DocumentTypesService
    {
        private static readonly ISender _mediator = DependencyInjection.Mediator;

        public static List<DocumentType> GetAllDocumentTypes()
        {
            var documentTypes = _mediator.Send(new GetAllDocumentTypesQuery()).Result;

            if (documentTypes.IsError != false)
            {
                ErrorHandlingService.HandleApiError(documentTypes);
                return null;
            }

            return documentTypes.Value.ToList();
        }
    }
}
