using LogicPOS.Api.Entities;
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
                    _documentTypes = GetAllDocumentTypes();
                }

                return _documentTypes;
            }
        }

        private static List<DocumentType> GetAllDocumentTypes()
        {
            var documentTypes = DependencyInjection.Mediator.Send(new GetAllDocumentTypesQuery()).Result;

            if (documentTypes.IsError != false)
            {
                ErrorHandlingService.HandleApiError(documentTypes);
                return null;
            }

            return documentTypes.Value.ToList();
        }
    

    }
}
