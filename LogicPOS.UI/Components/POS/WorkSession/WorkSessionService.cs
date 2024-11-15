using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.WorkSessions.GetAllWorkSessionPeriods;
using LogicPOS.Api.Features.WorkSessions.GetLastWorkSessionPeriod;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace LogicPOS.UI.Services
{
    public static class WorkSessionService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<ISender>();

        public static bool DayIsOpen()
        {
            var lastWorkSessionPeriod = GetLastWorkSessionPeriod();

            if (lastWorkSessionPeriod is null)
            {
                return false;
            }

            if (lastWorkSessionPeriod.Type == WorkSessionPeriodType.Terminal)
            {
                return true;
            }

            return lastWorkSessionPeriod.Status == WorkSessionPeriodStatus.Open;

        }

        public static bool TerminalSessionIsOpen()
        {
            var lastWorkSessionPeriod = GetLastWorkSessionPeriod(true);

            if (lastWorkSessionPeriod is null)
            {
                return false;
            }

            return lastWorkSessionPeriod.Status == WorkSessionPeriodStatus.Open;
        }

        private static WorkSessionPeriod GetLastWorkSessionPeriod(bool terminalSession = false)
        {
            var getResult = _mediator.Send(new GetLastWorkSessionPeriodQuery(terminalSession)).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult.FirstError, true);
                return null;
            }

            return getResult.Value;

        }

        public static bool HasAnyTerminalSessionOpen()
        {
            var query = new GetAllWorkSessionPeriodsQuery(WorkSessionPeriodType.Terminal, WorkSessionPeriodStatus.Open);
            var getResult = _mediator.Send(query).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult.FirstError, true);
                return false;
            }

            return getResult.Value.Any();
        }


    }
}
