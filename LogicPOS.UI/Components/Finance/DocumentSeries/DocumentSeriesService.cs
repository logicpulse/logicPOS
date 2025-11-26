using LogicPOS.Api.Features.Finance.Documents.Series.CreateDefaultSeries;
using LogicPOS.UI.Errors;
using System;

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
    }
}
