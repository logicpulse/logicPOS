using ErrorOr;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Customers.GetCurrentAccountPdf;
using LogicPOS.Api.Features.Reports.GetArticleReportPdf;
using LogicPOS.Api.Features.Reports.GetArticleTotalSoldReportPdf;
using LogicPOS.Api.Features.Reports.GetCompanyBillingReportPdf;
using LogicPOS.Api.Features.Reports.GetCustomerReportPdf;
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
using LogicPOS.Api.Features.Reports.GetSalesBySubFamilyDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByTaxGroupDetailedReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByVatAndArticleClassReportPdf;
using LogicPOS.Api.Features.Reports.GetSalesByVatAndArticleTypeReportPdf;
using LogicPOS.Api.Features.Reports.GetStockByArticleGainReportPdf;
using LogicPOS.Api.Features.Reports.GetStockByArticleReportPdf;
using LogicPOS.Api.Features.Reports.GetStockBySupplierReportPdfReportPdf;
using LogicPOS.Api.Features.Reports.GetStockMovementReportPdf;
using LogicPOS.Api.Features.Reports.GetStockReportPdf;
using LogicPOS.Api.Features.Reports.GetSuppliersReportPdf;
using LogicPOS.Api.Features.Reports.POS.DeletedOrders.GetDeletedOrdersReportPdf;
using LogicPOS.Api.Features.Reports.POS.SalesByCommission.GetSalesByCommissionReportPdf;
using LogicPOS.Api.Features.Reports.POS.SalesByPlace.GetSalesByPlaceDetailedReportPdf;
using LogicPOS.Api.Features.Reports.POS.SalesByTable.GetSalesByTableDetailedReportPdf;
using LogicPOS.Api.Features.Reports.POS.SalesByTerminal.GetSalesByTerminalDetailedReportPdf;
using LogicPOS.Api.Features.Reports.POS.SalesByTerminal.GetSalesByTerminalReportPdf;
using LogicPOS.Api.Features.Reports.SystemAudits.GetSystemAuditsReportPdf;
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

        public static void ShowDeletedOrdersReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetDeletedOrdersReportPdfQuery(startDate, endDate));
        }

        public static void ShowSalesByDocumentTypeReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByDocumentTypeReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByDateReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByDateReportPdfQuery(startDate, endDate,documentType, terminalId));
        }

        private static void ShowReport(IRequest<ErrorOr<TempFile>> query)
        {
            var result = _mediator.Send(query).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return;
            }

            LogicPOSPDFViewer.ShowPDF(result.Value.Path, result.Value.Name);
        }
        public static void ShowSalesByUserReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByEmployeeReportPdfQuery(startDate, endDate, documentType,terminalId));
        }

        public static void ShowSalesByTerminalReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByTerminalReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByCustomerReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByCustomerReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByPaymentMethodReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByPaymentMethodReportPdfQuery(startDate, endDate,documentType,terminalId));
        }

        public static void ShowSalesByPaymentConditionReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByPaymentConditionReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByCurrencyReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByCurrencyReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowArticleTotalSoldReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetArticleTotalSoldReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByCountryReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByCountryReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByVatAndArticleTypeReport(DateTime startDate, DateTime endDate, Guid? TaxId = null)
        {
            ShowReport(new GetSalesByVatAndArticleTypeReportPdfQuery(startDate, endDate, TaxId));
        }

        public static void ShowSalesByVatAndArticleClassReport(DateTime startDate, DateTime endDate, Guid? TaxId = null)
        {
            ShowReport(new GetSalesByVatAndArticleClassReportPdfQuery(startDate, endDate, TaxId));
        }

        public static void ShowDetailedSalesByCustomerReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByCustomerDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowCustomerBalanceDetailsReport(DateTime startDate, DateTime endDate, Guid customerId)
        {
            ShowReport(new GetCustomerCurrentAccountPdfQuery() { StartDate = startDate, EndDate = endDate, CustomerId = customerId });
        }

        public static void ShowSalesByDocumentDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByDocumentTypeDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByDateDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByDateDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByUserDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByEmployeeDetailedReportPdfQuery(startDate, endDate, documentType,terminalId));
        }

        public static void ShowSalesByTerminalDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByTerminalDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByPaymentConditionDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByPaymentConditionDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByPaymentMethodDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByPaymentMethodDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByCurrencyDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByCurrencyDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByCountryDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByCountryDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByFamilyDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByFamilyDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesBySubfamilyDetailsReport(GetSalesBySubFamilyDetailedReportPdfQuery query)
        {
            ShowReport(query);
        }

        public static void ShowSalesByPlaceDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByPlaceDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByTableDetailsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetSalesByTableDetailedReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowSalesByVatGroupDetailsReport(DateTime startDate, DateTime endDate, Guid? taxId = null)
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

        public static void ShowSuppliersReport()
        {
            ShowReport(new GetSuppliersReportPdfQuery());
        }

        public static void ShowCommissionsReport(DateTime startDate, DateTime endDate)
        {
            ShowReport(new GetSalesByCommissionReportPdfQuery(startDate, endDate));
        }

        public static void ShowStockMovementsReport(DateTime startDate, DateTime endDate, string documentType = null, Guid? terminalId = null)
        {
            ShowReport(new GetStockMovementsReportPdfQuery(startDate, endDate, documentType, terminalId));
        }

        public static void ShowStockByWarehouseReport(DateTime startDate, DateTime endDate, Guid? articleId, Guid? warehouseId, string serialNumber=null)
        {
            ShowReport(new GetStockByWarehouseReportPdfQuery(startDate, endDate, articleId, warehouseId, serialNumber));
        }

        public static void ShowStockByArticleReport(DateTime startDate, DateTime endDate, Guid? articleId=null)
        {
            ShowReport(new GetStockByArticleReportPdfQuery(startDate, endDate, articleId));
        }

        public static void ShowStockBySupplierReport(DateTime startDate, DateTime endDate, Guid? supplierId = null, string documentNumber = null)
        {
            ShowReport(new GetStockBySupplierReportPdfQuery(startDate, endDate, supplierId, documentNumber));
        }

        public static void ShowStockByArticleGainReport(DateTime startDate, DateTime endDate, Guid? articleId, Guid? customerId)
        {
            ShowReport(new GetStockByArticleGainReportPdfQuery(startDate, endDate, articleId, customerId));
        }

        public static void ShowSystemAuditsReport(DateTime startDate, DateTime endDate, Guid? terminalId)
        {
            ShowReport(new GetSystemAuditsReportPdfQuery(startDate, endDate, terminalId));
        }
    }
}
