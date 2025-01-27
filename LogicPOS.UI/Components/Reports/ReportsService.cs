using ErrorOr;
using LogicPOS.Api.Features.Reports.GetCompanyBillingReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByDateReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByDocumentTypeReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByEmployeeReportPdf;
using LogicPOS.UI.Errors;
using LogicPOS.UI.PDFViewer;
using MediatR;
using System;

namespace LogicPOS.UI.Services
{
    public static class ReportsService
    {
        private static readonly ISender _mediator = DependencyInjection.Mediator;

        public static void ShowCompanyBillingReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetCompanyBillingReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByDocumentTypeReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByDocumentTypeReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByDateReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByDateReportPdfQuery(startDate, endDate));
        }

        private static void ShowReport(IRequest<ErrorOr<string>> query)
        {
            var result = _mediator.Send(query).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return;
            }
            LogicPOSPDFViewer.ShowPDF(result.Value);
        }

        public static void ShowSalesByUserReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByEmployeeReportPdfQuery(startDate, endDate));
        }
    }
}
