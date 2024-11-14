using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.FiscalYears.GetCurrentFiscalYear;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.FiscalYears
{
    public static class FiscalYearService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();
        public static FiscalYear CurrentFiscalYear { get; private set; }
        public static bool HasFiscalYear => CurrentFiscalYear != null;
       
        public static async Task<ErrorOr<FiscalYear>> InitializeAsync(CancellationToken ct = default)
        {
            var getFiscalYear = await _mediator.Send(new GetCurrentFiscalYearQuery(), ct);

            if (getFiscalYear.IsError)
            {
                return getFiscalYear;
            }

            CurrentFiscalYear = getFiscalYear.Value;

            return CurrentFiscalYear;
        }

        public static void ShowOpenFiscalYearAlert()
        {
            CustomAlerts.Warning(POSWindow.Instance)
                       .WithSize(new Size(600, 400))
                       .WithTitleResource("global_warning")
                       .WithMessage(GeneralUtils.GetResourceByName("global_warning_open_fiscal_year"))
                       .ShowAlert();
        }
    }

}
