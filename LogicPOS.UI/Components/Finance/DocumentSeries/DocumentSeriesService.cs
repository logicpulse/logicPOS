using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Documents.Series.GetAllDocumentSeries;
using LogicPOS.Api.Features.Finance.Documents.Series.CreateAgtSeries;
using LogicPOS.Api.Features.Finance.Documents.Series.CreateDefaultSeries;
using LogicPOS.UI.Errors;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.DocumentSeries
{
    public static class DocumentSeriesService
    {
        public static bool CreateDefaultSeriesForFiscalYear(Guid fiscalYearId)
        {
            var result = DependencyInjection.Mediator.Send(new CreateDefaultSeriesCommand(fiscalYearId)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            return true;
        }

        public static bool CreateAgtSeries(CreateAgtSeriesCommand command)
        {
            var result = DependencyInjection.Mediator.Send(command).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            return true;
        }

    }
}
