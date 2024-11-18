using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.WorkSessions.CloseAllSessions;
using LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodDay;
using LogicPOS.Api.Features.WorkSessions.GetAllWorkSessionPeriods;
using LogicPOS.Api.Features.WorkSessions.GetLastWorkSessionPeriod;
using LogicPOS.Api.Features.WorkSessions.OpenWorkSessionPeriodDay;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

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

        public static IEnumerable<WorkSessionPeriod> GetOpenTerminalSessions()
        {
            var query = new GetAllWorkSessionPeriodsQuery(WorkSessionPeriodType.Terminal, WorkSessionPeriodStatus.Open);
            var getResult = _mediator.Send(query).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult.FirstError, true);
            }

            return getResult.Value;
        }

        public static bool OpenDay()
        {
            var openDayResult = _mediator.Send(new OpenWorkSessionPeriodDayCommand()).Result;

            if (openDayResult.IsError)
            {
                ErrorHandlingService.HandleApiError(openDayResult.FirstError);
                return false;
            }

            return true;
        }

        public static bool CloseDay()
        {
            var openDayResult = _mediator.Send(new CloseWorkSessionPeriodDayCommand()).Result;

            if (openDayResult.IsError)
            {
                ErrorHandlingService.HandleApiError(openDayResult.FirstError);
                return false;
            }

            return true;
        }

        public static bool CloseTerminalAllSessions()
        {
            var openDayResult = _mediator.Send(new CloseAllWorkSessionsCommand()).Result;

            if (openDayResult.IsError)
            {
                ErrorHandlingService.HandleApiError(openDayResult.FirstError);
                return false;
            }

            return true;
        }
    }
}
