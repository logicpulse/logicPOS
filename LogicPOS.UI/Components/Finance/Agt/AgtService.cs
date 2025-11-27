using LogicPOS.Api.Features.Finance.Agt.Common;
using LogicPOS.Api.Features.Finance.Agt.GetAgtDocumentById;
using LogicPOS.Api.Features.Finance.Agt.GetContributorByNif;
using LogicPOS.Api.Features.Finance.Agt.ListSeries;
using LogicPOS.Api.Features.Finance.Agt.RegisterDocument;
using LogicPOS.Api.Features.Finance.Agt.RequestSeries;
using LogicPOS.Api.Features.Finance.Agt.UpdateDocumentValidationStatus;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.Agt
{
    public static class AgtService
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

        public static List<Api.Features.Finance.Agt.ListSeries.AgtSeriesInfo> ListOnlineSeries()
        {
            var result = DependencyInjection.Mediator.Send(new ListAgtSeriesQuery()).Result;
            if (result.IsError != false)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value.ToList();
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

        public static string[] EligibleDocumentTypes { get; } = new string[] { "FT", "FR", "RG", "NC", "ND" };

        public static Api.Features.Finance.Agt.RequestSeries.AgtSeriesInfo? RequestSeries(RequestSeriesCommand command)
        {
            var result = DependencyInjection.Mediator.Send(command).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }
    }
}
