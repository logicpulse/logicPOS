using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes;
using LogicPOS.Api.Features.Finance.Documents.Types.Common;
using LogicPOS.Api.Features.Finance.Documents.Types.GetActiveDocumentTypes;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.DocumentTypes
{
    public static class DocumentTypesService
    {
        public static List<AutoCompleteLine> AutocompleteLines => GetActive()?.Where(dc => dc.SaftDocumentType != SaftDocumentType.Payments).Select(pc => new AutoCompleteLine
        {
            Id = pc.Id,
            Name = pc.Designation
        }).ToList();
 
        public static DocumentType Default => GetActive()?.OrderBy(dt => dt.Order).FirstOrDefault();

        public static List<DocumentType> GetAll()
        {
            var documentTypes = DependencyInjection.Mediator.Send(new GetAllDocumentTypesQuery()).Result;

            if (documentTypes.IsError != false)
            {
                ErrorHandlingService.HandleApiError(documentTypes);
                return null;
            }

            return documentTypes.Value.ToList();
        }

        public static List<DocumentType> GetActive()
        {
            var documentTypes = DependencyInjection.Mediator.Send(new GetActiveDocumentTypesQuery()).Result;
            if (documentTypes.IsError != false)
            {
                ErrorHandlingService.HandleApiError(documentTypes);
                return null;
            }

            return documentTypes.Value.ToList();
        }

        public static DocumentType GetById(System.Guid id)
        {
            return GetAll().FirstOrDefault(dt => dt.Id == id);
        }
    }
}
