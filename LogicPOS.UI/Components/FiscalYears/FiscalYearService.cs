using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.FiscalYears.GetCurrentFiscalYear;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;

namespace LogicPOS.UI.Components.FiscalYears
{
    public static class FiscalYearService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public static FiscalYear CurrentFiscalYear { get; private set; }

        public static void Initialize()
        {
            var getFiscalYear = _mediator.Send(new GetCurrentFiscalYearQuery()).Result;

            if (getFiscalYear.IsError)
            {
                ErrorHandlingService.HandleApiError(getFiscalYear.FirstError);
                return;
            }

            CurrentFiscalYear = getFiscalYear.Value;
        }

        public static void ShowOpenFiscalYearAlert()
        {
            CustomAlerts.Warning(POSWindow.Instance)
                       .WithSize(new Size(600, 400))
                       .WithTitleResource("global_warning")
                       .WithMessage(GeneralUtils.GetResourceByName("global_warning_open_fiscal_year"))
                       .ShowAlert();
        }

        public static bool HasFiscalYear()
        {
            if (CurrentFiscalYear != null)
            {
                return true;
            }

            Initialize();

            return CurrentFiscalYear != null;
        }
    }

}
