using LogicPOS.Api.Entities;
using LogicPOS.Api.Entities.Enums;
using LogicPOS.Api.Features.WorkSessions;
using LogicPOS.Api.Features.WorkSessions.CloseAllSessions;
using LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodDay;
using LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodSession;
using LogicPOS.Api.Features.WorkSessions.GetAllWorkSessionPeriods;
using LogicPOS.Api.Features.WorkSessions.Movements.AddCashDrawerInMovement;
using LogicPOS.Api.Features.WorkSessions.Movements.AddCashDrawerOutMovement;
using LogicPOS.Api.Features.WorkSessions.Movements.GetTotalCashInDrawer;
using LogicPOS.Api.Features.WorkSessions.OpenWorkSessionPeriodDay;
using LogicPOS.Api.Features.WorkSessions.Periods.DayIsOpen;
using LogicPOS.Api.Features.WorkSessions.Periods.OpenTerminalSession;
using LogicPOS.Api.Features.WorkSessions.Periods.TerminalIsOpen;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Services
{
    public static class WorkSessionService
    {
        private static readonly ISender _mediator = DependencyInjection.Mediator;

        public static bool DayIsOpen()
        {
            var getResult = _mediator.Send(new DayIsOpenQuery()).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult, true);
                return false;
            }

            return getResult.Value;
        }

        public static bool TerminalIsOpen()
        {
            var getResult = _mediator.Send(new TerminalIsOpenQuery(TerminalService.Terminal.Id)).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult, true);
                return false;
            }

            return getResult.Value;
        }

        public static IEnumerable<WorkSessionPeriod> GetOpenTerminalSessions()
        {
            var query = new GetAllWorkSessionPeriodsQuery(WorkSessionPeriodType.Terminal, WorkSessionPeriodStatus.Open);
            var getResult = _mediator.Send(query).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult, true);
            }

            return getResult.Value;
        }

        public static bool OpenDay()
        {
            var openDayResult = _mediator.Send(new OpenWorkSessionPeriodDayCommand()).Result;

            if (openDayResult.IsError)
            {
                ErrorHandlingService.HandleApiError(openDayResult);
                return false;
            }

            return true;
        }

        public static bool CloseDay()
        {
            var openDayResult = _mediator.Send(new CloseWorkSessionPeriodDayCommand()).Result;

            if (openDayResult.IsError)
            {
                ErrorHandlingService.HandleApiError(openDayResult);
                return false;
            }

            return true;
        }

        public static bool CloseAllTerminalSessions()
        {
            var closeAllTerminalSessionsResult = _mediator.Send(new CloseAllWorkSessionsCommand()).Result;

            if (closeAllTerminalSessionsResult.IsError)
            {
                ErrorHandlingService.HandleApiError(closeAllTerminalSessionsResult);
                return false;
            }

            return true;
        }

        public static decimal GetTotalCashInCashDrawer()
        {
            var terminalId = TerminalService.Terminal.Id;
            var result = _mediator.Send(new GetTotalCashInDrawerQuery(terminalId)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return -1;
            }

            return result.Value ?? -1;
        }

        public static bool OpenTerminalSession(decimal amount, string notes)
        {
            var command = new OpenTerminalSessionCommand(amount, notes);
            var result = _mediator.Send(command).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }
            OpenTotal= amount;
            return true;
        }

        public static bool CloseTerminalSession(decimal amount, string notes)
        {
            var command = new CloseTerminalSessionCommand(amount, notes);
            var result = _mediator.Send(command).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }
            CloseTotal = amount;
            return true;
        }

        public static bool AddCashDrawerInMovement(decimal amount, string notes)
        {
            var command = new AddCashDrawerInMovementCommand(amount, notes);
            var result = _mediator.Send(command).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }
            CashDrawerInTotal += amount;
            return true;
        }

        public static bool AddCashDrawerOutMovement(decimal amount, string notes)
        {
            var command = new AddCashDrawerOutMovementCommand(amount, notes);
            var result = _mediator.Send(command).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }
            CashDrawerOutTotal += amount;
            return true;
        }

        public static WorkSessionPeriod GetLastWorkSessionByTerminal()
        {
            var command = new GetLastWorkSessionByTerminalIdQuery(TerminalService.Terminal.Id);
            
            var result = _mediator.Send(command).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value;
        }

        public static decimal OpenTotal { get; set; }
        public static decimal CloseTotal { get; set; }
        public static decimal CashDrawerInTotal { get; set; }
        public static decimal CashDrawerOutTotal { get; set; }
    }
}
