using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Finance.FiscalYears.CloseFiscalYear;
using LogicPOS.Api.Features.Finance.FiscalYears.GetFiscalYearCreationData;
using LogicPOS.Api.Features.FiscalYears.GetCurrentFiscalYear;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
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

        public static bool CloseCurrentFiscalYear()
        {
            if (HasActiveFiscalYear() == false)
            {
                return false;
            }

            var result = DependencyInjection.Mediator.Send(new CloseFiscalYearCommand(CurrentFiscalYear.Id)).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return false;
            }

            _currentFiscalYear = null;

            return true;
        }
       
        public static void ShowOpenFiscalYearAlert(Gtk.Window parent)
        {
            CustomAlerts.Warning(parent)
                       .WithSize(new Size(600, 400))
                       .WithTitleResource("global_warning")
                       .WithMessage(GeneralUtils.GetResourceByName("global_warning_open_fiscal_year"))
                       .ShowAlert();
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
