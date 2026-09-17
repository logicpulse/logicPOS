using LogicPOS.Api.Features.Reports.SalesByDate.GetMonthlySalesReportData;
using LogicPOS.Api.Features.Reports.SalesByDate.GetSalesTotalForDay;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Windows.BackOffice.Dashboard
{
    public static class DashboardDataService
    {
        public static MonthlySalesReportData GetMonthlySalesReportData(int year)
        {
            var getResult = DependencyInjection.Mediator.Send(new GetMonthlySalesReportDataQuery(year)).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult, source: BackOfficeWindow.Instance);
                return null;
            }

            return getResult.Value;
        }

        public static List<int> GetAvailableYearsForSalesReport()
        {
            var data = GetMonthlySalesReportData(DateTime.Now.Year);
            if (data == null)
            {
                return null;
            }

            return data.Years;
        }

        public static TotalSalesForDay GetTotalSalesForDay(DateTime day)
        {
            var getResult = DependencyInjection.Mediator.Send(new GetSalesTotalForDayQuery(day)).Result;

            if (getResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getResult, source: BackOfficeWindow.Instance);
                return null;
            }

            return getResult.Value;
        }

    }
}