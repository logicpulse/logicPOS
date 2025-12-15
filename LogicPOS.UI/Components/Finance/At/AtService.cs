using LogicPOS.Api.Features.Finance.At.RegisterSeries;
using LogicPOS.UI.Errors;
using System;

namespace LogicPOS.UI.Components.Finance.At
{
    public static class AtService
    {
        public static AtSeriesInfo? RegisterSeries(Guid id)
        {
            var command = new RegisterSeriesCommand(id);
            var result = DependencyInjection.Mediator.Send(command).Result;

            if (result.IsError != false)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }
    }
}
