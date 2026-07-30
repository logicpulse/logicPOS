using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.FiscalYears.CloseFiscalYear;
using LogicPOS.Api.Features.Finance.FiscalYears.GetFiscalYearCreationData;
using LogicPOS.Api.Features.FiscalYears.GetCurrentFiscalYear;
using LogicPOS.Globalization;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using LogicPOS.UI.Services;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace LogicPOS.UI.Components.FiscalYears
{
    public static class FiscalYearsService
    {
        private static FiscalYear _currentFiscalYear;
        public static FiscalYear CurrentFiscalYear
        {
            get
            {
                if (_currentFiscalYear == null)
                {
                    _currentFiscalYear = GetCurrentFiscalYear();
                }
                return _currentFiscalYear;
            }
        }

        private static FiscalYear GetCurrentFiscalYear()
        {
            var getFiscalYear = DependencyInjection.Mediator.Send(new GetCurrentFiscalYearQuery()).Result;

            if (getFiscalYear.IsError)
            {
                ErrorHandlingService.HandleApiError(getFiscalYear);
                return null;
            }

            return getFiscalYear.Value;
        }

        public static bool CloseCurrentFiscalYear(Gtk.Window parent = null)
        {
            if (HasActiveFiscalYear() == false)
            {
                return false;
            }

            var fiscalYearId = CurrentFiscalYear.Id;
            var result = DependencyInjection.Mediator
                .Send(new CloseFiscalYearCommand(fiscalYearId, forceAtCommunication: true))
                .Result;

            if (result.IsError)
            {
                if (SystemInformationService.SystemInformation?.IsPortugal == true)
                {
                    var retry = CustomAlerts.Question(parent)
                        .WithSize(new Size(600, 400))
                        .WithTitleResource("global_warning")
                        .WithMessage(
                            "Não foi possível comunicar com a AT ao fechar o ano fiscal.\n\n" +
                            $"{result.FirstError.Description}\n\n" +
                            "Deseja fechar o ano fiscal mesmo assim (sem comunicação com a AT)?")
                        .ShowAlert();

                    if (retry != Gtk.ResponseType.Yes)
                    {
                        return false;
                    }

                    result = DependencyInjection.Mediator
                        .Send(new CloseFiscalYearCommand(fiscalYearId, forceAtCommunication: false))
                        .Result;

                    if (result.IsError)
                    {
                        ErrorHandlingService.HandleApiError(result, source: parent);
                        return false;
                    }
                }
                else
                {
                    ErrorHandlingService.HandleApiError(result, source: parent);
                    return false;
                }
            }

            _currentFiscalYear = null;

            return true;
        }
       
        public static void ShowOpenFiscalYearAlert(Gtk.Window parent)
        {
            CustomAlerts.Warning(parent)
                       .WithSize(new Size(600, 400))
                       .WithTitleResource("global_warning")
                       .WithMessage(LocalizedString.Instance["global_warning_open_fiscal_year"])
                       .ShowAlert();
        }


        public static bool ConfirmProceedWhenActiveFiscalYearDiffersFromCalendarYear(Gtk.Window parent)
        {
            var fiscalYear = CurrentFiscalYear;
            if (fiscalYear == null || fiscalYear.Year == DateTime.Now.Year)
            {
                return true;
            }

            return CustomAlerts.Question(parent)
                .WithSize(new Size(600, 400))
                .WithTitleResource("global_warning")
                .WithMessage(
                    $"O ano fiscal activo ({fiscalYear.Year}) não coincide com o ano actual ({DateTime.Now.Year}).\n\n" +
                    "Deseja continuar mesmo assim?")
                .ShowAlert() == Gtk.ResponseType.Yes;
        }

        public static FiscalYearCreationData? GetCreationRelevantData()
        {
            var result = DependencyInjection.Mediator.Send(new GetFiscalYearCreationDataQuery()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return null;
            }

            return result.Value;
        }

        public static bool HasActiveFiscalYear() => CurrentFiscalYear != null;
    }

}
