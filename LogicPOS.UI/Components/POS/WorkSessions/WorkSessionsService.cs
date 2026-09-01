using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetDayReportData;
using LogicPOS.Api.Features.POS.WorkSessions.Movements.GetTerminalSessionReportData;
using LogicPOS.Api.Features.WorkSessions;
using LogicPOS.Api.Features.WorkSessions.CloseAllSessions;
using LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodDay;
using LogicPOS.Api.Features.WorkSessions.CloseWorkSessionPeriodSession;
using LogicPOS.Api.Features.WorkSessions.GetAllWorkSessionPeriods;
using LogicPOS.Api.Features.WorkSessions.GetLastClosedDay;
using LogicPOS.Api.Features.WorkSessions.Movements.AddCashDrawerInMovement;
using LogicPOS.Api.Features.WorkSessions.Movements.AddCashDrawerOutMovement;
using LogicPOS.Api.Features.WorkSessions.Movements.GetTotalCashInDrawer;
using LogicPOS.Api.Features.WorkSessions.OpenWorkSessionPeriodDay;
using LogicPOS.Api.Features.WorkSessions.Periods.DayIsOpen;
using LogicPOS.Api.Features.WorkSessions.Periods.OpenTerminalSession;
using LogicPOS.Api.Features.WorkSessions.Periods.TerminalIsOpen;
using LogicPOS.UI.Components.Terminals;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Services
{
    public static class WorkSessionsService
    {
        private static readonly object TerminalOpenCacheSync = new object();
        private static bool? _terminalIsOpenCached;
        private static DateTime _terminalIsOpenCachedUtc = DateTime.MinValue;
        private static readonly TimeSpan TerminalIsOpenCacheTtl = TimeSpan.FromSeconds(30);

        public static bool DayIsOpen()
        {
            var getResult = DependencyInjection.Mediator.Send(new DayIsOpenQuery()).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult, true);
                return false;
            }

            return getResult.Value;
        }

        public static bool TerminalIsOpen()
        {
            lock (TerminalOpenCacheSync)
            {
                if (_terminalIsOpenCached.HasValue
                    && DateTime.UtcNow - _terminalIsOpenCachedUtc < TerminalIsOpenCacheTtl)
                {
                    return _terminalIsOpenCached.Value;
                }
            }

            var getResult = DependencyInjection.Mediator.Send(new TerminalIsOpenQuery(TerminalService.Terminal.Id)).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult, true);
                SetTerminalIsOpenCache(false);
                return false;
            }

            SetTerminalIsOpenCache(getResult.Value);
            return getResult.Value;
        }

        /// <summary>
        /// Avoids repeated TerminalIsOpen GETs (e.g. POSWindow.UpdateUI + SaleOptionsPanel).
        /// Invalidate on open/close session.
        /// </summary>
        public static void SetTerminalIsOpenCache(bool isOpen)
        {
            lock (TerminalOpenCacheSync)
            {
                _terminalIsOpenCached = isOpen;
                _terminalIsOpenCachedUtc = DateTime.UtcNow;
            }
        }

        public static void InvalidateTerminalIsOpenCache()
        {
            lock (TerminalOpenCacheSync)
            {
                _terminalIsOpenCached = null;
                _terminalIsOpenCachedUtc = DateTime.MinValue;
            }
        }

        public static List<WorkSessionPeriod> GetOpenTerminalSessions()
        {
            var query = new GetOpenTerminalSessionsQuery();
            var getResult = DependencyInjection.Mediator.Send(query).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult);
                return null;
            }

            return getResult.Value.ToList();
        }

        public static bool OpenDay()
        {
            var openDayResult = DependencyInjection.Mediator.Send(new OpenWorkSessionPeriodDayCommand()).Result;

            if (openDayResult.IsError)
            {
                ErrorHandlingService.HandleApiError(openDayResult);
                return false;
            }

            return true;
        }

        public static bool CloseDay()
        {
            var openDayResult = DependencyInjection.Mediator.Send(new CloseWorkSessionPeriodDayCommand()).Result;

            if (openDayResult.IsError)
            {
                ErrorHandlingService.HandleApiError(openDayResult);
                return false;
            }

            return true;
        }

        public static bool CloseAllTerminalSessions()
        {
            var closeAllTerminalSessionsResult = DependencyInjection.Mediator.Send(new CloseAllWorkSessionsCommand()).Result;

            if (closeAllTerminalSessionsResult.IsError)
            {
                ErrorHandlingService.HandleApiError(closeAllTerminalSessionsResult);
                return false;
            }

            SetTerminalIsOpenCache(false);
            return true;
        }

        public static decimal GetTotalCashInCashDrawer()
        {
            var terminalId = TerminalService.Terminal.Id;
            var result = DependencyInjection.Mediator.Send(new GetTotalCashInDrawerQuery(terminalId)).Result;

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
            var result = DependencyInjection.Mediator.Send(command).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }
            OpenTotal = amount;
            SetTerminalIsOpenCache(true);
            return true;
        }

        public static bool CloseTerminalSession(decimal amount, string notes)
        {
            var command = new CloseTerminalSessionCommand(amount, notes);
            var result = DependencyInjection.Mediator.Send(command).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }
            CloseTotal = amount;
            SetTerminalIsOpenCache(false);
            return true;
        }

        public static bool AddCashDrawerInMovement(decimal amount, string notes)
        {
            var command = new AddCashDrawerInMovementCommand(amount, notes);
            var result = DependencyInjection.Mediator.Send(command).Result;
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
            var result = DependencyInjection.Mediator.Send(command).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }
            CashDrawerOutTotal += amount;
            return true;
        }

        public static WorkSessionPeriod GetTerminalLastWorkSession()
        {
            var command = new GetLastWorkSessionByTerminalIdQuery(TerminalService.Terminal.Id);

            var result = DependencyInjection.Mediator.Send(command).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }
            return result.Value;
        }
        public static WorkSessionPeriod GetLastClosedDay()
        {
            var result = DependencyInjection.Mediator.Send(new GetLastClosedDayQuery()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }


        public static DayReportData GetLastClosedTerminalSessionReportData()
        {
            var lastClosedTerminalSession = GetTerminalLastWorkSession();

            if (lastClosedTerminalSession == null)
            {
                return null;
            }

            var result = DependencyInjection.Mediator.Send(new GetTerminalSessionReportDataQuery(lastClosedTerminalSession.Id)).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }

        public static DayReportData GetLastClosedDayReportData()
        {
            var lastClosedDay = GetLastClosedDay();

            if (lastClosedDay == null)
            {
                return null;
            }

            var result = DependencyInjection.Mediator.Send(new GetDayReportDataQuery(lastClosedDay.Id)).Result;
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
