using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.DocumentTypes.GetAllDocumentTypes;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.DocumentTypes
{
    public static class DocumentTypesService
    {
        private static DocumentType _default;
        private static List<DocumentType> _documentTypes;

        public static DocumentType Default
        {
            get
            {
                if( _default == null)
                {
                    _default = DocumentTypes.FirstOrDefault(type => type.Designation == "Fatura");
                }

                return _default;
            }
        }
        public static List<DocumentType> DocumentTypes
        {
            get
            {
                if (_documentTypes == null)
                {
                    _documentTypes = GetAll();
                }

                return _documentTypes;
            }
        }

        public static List<AutoCompleteLine> AutocompleteLines => DocumentTypes.Select(pc => new AutoCompleteLine
        {
            Id = pc.Id,
            Name = pc.Designation
        }).ToList();

        private static List<DocumentType> GetAll()
        {
            var documentTypes = DependencyInjection.Mediator.Send(new GetAllDocumentTypesQuery()).Result;

            if (documentTypes.IsError != false)
            {
                ErrorHandlingService.HandleApiError(documentTypes);
                return null;
            }

            return documentTypes.Value.ToList();
        }
    
        public static DocumentType GetById(System.Guid id)
        {
            return DocumentTypes.FirstOrDefault(dt => dt.Id == id);
        }
    }
}
