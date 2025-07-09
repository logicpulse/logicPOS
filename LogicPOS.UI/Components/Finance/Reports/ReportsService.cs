using ErrorOr;
using LogicPOS.Api.Features.Customers.GetCurrentAccountPdf;
using LogicPOS.Api.Features.Reports.GetArticleReportPdf;
using LogicPOS.Api.Features.Reports.GetArticleTotalSoldReportPdf;
using LogicPOS.Api.Features.Reports.GetCompanyBillingReportPdf;
using LogicPOS.Api.Features.Reports.GetCustomerReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByCommissionReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByCountryDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByCountryReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByCurrencyDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByCurrencyReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByCustomerDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByCustomerReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByDateDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByDateReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByDocumentTypeDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByDocumentTypeReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByEmployeeDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByEmployeeReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByFamilyDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByPaymentConditionDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByPaymentConditionReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByPaymentMethodDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByPaymentMethodReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByPlaceDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesBySubFamilyDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByTableDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByTaxGroupDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByTerminalDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByTerminalReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByVatAndArticleClassReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByVatAndArticleTypeReportPdf;
using LogicPOS.Api.Features.Reports.GetStockByArticleGainReportPdf;
using LogicPOS.Api.Features.Reports.GetStockByArticleReportPdf;
using LogicPOS.Api.Features.Reports.GetStockBySupplierReportPdfReportPdf;
using LogicPOS.Api.Features.Reports.GetStockMovementReportPdf;
using LogicPOS.Api.Features.Reports.GetStockReportPdf;
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

        public static void ShowSalesByTerminalReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByTerminalReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByCustomerReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByCustomerReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByPaymentMethodReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByPaymentMethodReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByPaymentConditionReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByPaymentConditionReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByCurrencyReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByCurrencyReportPdfQuery(startDate, endDate));
        }

        public static void ShowArticleTotalSoldReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetArticleTotalSoldReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByCountryReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByCountryReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByVatAndArticleTypeReport(DateTime startDate, DateTime endDate, Guid TaxId = new Guid())
        {
            ShowReport(new GetSalesByVatAndArticleTypeReportPdfQuery(startDate, endDate, TaxId));
        }

        public static void ShowSalesByVatAndArticleClassReport(DateTime startDate, DateTime endDate, Guid TaxId = new Guid())
        {
            ShowReport(new GetSalesByVatAndArticleClassReportPdfQuery(startDate, endDate, TaxId));
        }

        public static void ShowDetailedSalesByCustomerReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByCustomerDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowCustomerBalanceDetailsReport(DateTime startDate, DateTime endDate, Guid customerId = new Guid())
        {
            ShowReport(new GetCustomerCurrentAccountPdfQuery() { StartDate = startDate, EndDate = endDate, CustomerId = customerId });
        }

        public static void ShowSalesByDocumentDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByDocumentTypeDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByDateDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByDateDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByUserDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByEmployeeDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByTerminalDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByTerminalDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByPaymentConditionDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByPaymentConditionDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByPaymentMethodDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByPaymentMethodDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByCurrencyDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByCurrencyDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByCountryDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByCountryDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByFamilyDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByFamilyDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesBySubfamilyDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesBySubFamilyDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByPlaceDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByPlaceDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByTableDetailsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByTableDetailedReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByVatGroupDetailsReport(DateTime startDate, DateTime endDate, Guid taxId = new Guid())
        {

            ShowReport(new GetSalesByTaxGroupDetailedReportPdfQuery(startDate, endDate, taxId));
        }

        public static void ShowArticlesReport()
        {
            ShowReport(new GetArticleReportPdfQuery());
        }

        public static void ShowCustomersReport()
        {
            ShowReport(new GetCustomerReportPdfQuery());
        }

        public static void ShowCommissionsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByCommissionReportPdfQuery(startDate, endDate));
        }

        public static void ShowStockMovementsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetStockMovementsReportPdfQuery(startDate, endDate));
        }

        public static void ShowStockByWarehouseReport(DateTime startDate, DateTime endDate, Guid articleId, Guid warehouseId, string serialNumber)
        {
            ShowReport(new GetStockByWarehouseReportPdfQuery(startDate, endDate, articleId, warehouseId, serialNumber));
        }

        public static void ShowStockByArticleReport(DateTime startDate, DateTime endDate, Guid articleId=new Guid())
        {
            ShowReport(new GetStockByArticleReportPdfQuery(startDate, endDate, articleId));
        }

        public static void ShowStockBySupplierReport(DateTime startDate, DateTime endDate, Guid supplierId = new Guid(), string documentNumber="")
        {
            ShowReport(new GetStockBySupplierReportPdfQuery(startDate, endDate, supplierId, documentNumber));
        }

        public static void ShowStockByArticleGainReport(DateTime startDate, DateTime endDate, Guid articleId = new Guid(), Guid customerId = new Guid())
        {
            ShowReport(new GetStockByArticleGainReportPdfQuery(startDate, endDate, articleId, customerId));
        }
    }
}
