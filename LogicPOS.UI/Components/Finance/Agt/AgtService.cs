using LogicPOS.Api.Features.Finance.Agt.Common;
using LogicPOS.Api.Features.Finance.Agt.GetAgtDocumentById;
using LogicPOS.Api.Features.Finance.Agt.GetContributorByNif;
using LogicPOS.Api.Features.Finance.Agt.RegisterDocument;
using LogicPOS.Api.Features.Finance.Agt.UpdateDocumentValidationStatus;
using LogicPOS.UI.Errors;
using System;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public class AgtService
    {
        public static Contributor GetAgtContributorInfo(string nif)
        {
            var contributor = DependencyInjection.Mediator.Send(new GetContributorByNifQuery(nif)).Result;

            if (contributor.IsError != false)
            {
                ErrorHandlingService.HandleApiError(contributor);
                return null;
            }

            return contributor.Value;
        }

        public static bool RegisterDocument(Guid documentId)
        {
            var command = new RegisterDocumentCommand(documentId);
            var result = DependencyInjection.Mediator.Send(command).Result;

            if (result.IsError != false)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            return true;
        }

        public static bool UpdateDocumentValidationStatus(Guid documentId)
        {
            var command = new UpdateDocumentValidationStatusCommand(documentId);
            var result = DependencyInjection.Mediator.Send(command).Result;

            if (result.IsError != false)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            return true;
        }
        
        public static AgtDocument GetAgtDocument(Guid documentId)
        {
            var result = DependencyInjection.Mediator.Send(new GetAgtDocumentByIdQuery(documentId)).Result;

            if (result.IsError != false)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }
    }
}
