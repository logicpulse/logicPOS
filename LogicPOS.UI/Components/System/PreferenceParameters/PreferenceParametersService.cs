using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Company;
using LogicPOS.Api.Features.Company.GetCompanyInformations;
using LogicPOS.Api.Features.PreferenceParameters.GetAllPreferenceParameters;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Services
{
    public static class PreferenceParametersService
    {
        private static IEnumerable<PreferenceParameter> _preferenceParameters;
        private static CompanyInformations _companyInformations;

        public static string GetPreferenceParameterValue(string token)
        {
            if (_preferenceParameters == null)
            {
                LoadPreferenceParameters(); 
            }

            var preferenceParameter = _preferenceParameters.FirstOrDefault(p => p.Token == token);

            return preferenceParameter?.Value;
        }

        private static void LoadPreferenceParameters()
        {
            var mediator = DependencyInjection.Mediator;
            var preferenceParameters = mediator.Send(new GetAllPreferenceParametersQuery()).Result;
           
            if (preferenceParameters.IsError)
            {
                CustomAlerts.ShowApiErrorAlert(null, preferenceParameters.FirstError);
            }
             
            _preferenceParameters = preferenceParameters.Value;
        }

        private static void LoadCompanyInformations()
        {
            var mediator = DependencyInjection.Mediator;
            var companyInformations = mediator.Send(new GetCompanyInformationsQuery()).Result;

            if (companyInformations.IsError)
            {
                ErrorHandlingService.HandleApiError(companyInformations, true);
                return;
            }

            _companyInformations = companyInformations.Value;
        }

        public static bool UseCachedImages => Convert.ToBoolean(GetPreferenceParameterValue("USE_CACHED_IMAGES"));
        public static bool UseEuropeanVatAutoComplete => Convert.ToBoolean(GetPreferenceParameterValue("USE_EUROPEAN_VAT_AUTOCOMPLETE"));
        public static bool UsePosPdfViewer => Convert.ToBoolean(GetPreferenceParameterValue("USE_POS_PDF_VIEWER"));
        public static bool PrintQrCode => Convert.ToBoolean(GetPreferenceParameterValue("PRINT_QRCODE"));
        public static bool PrintTicket => Convert.ToBoolean(GetPreferenceParameterValue("TICKET_PRINT_TICKET"));
        public static bool CheckStocks => Convert.ToBoolean(GetPreferenceParameterValue("CHECK_STOCKS"));
        public static bool CheckStocksMessage => Convert.ToBoolean(GetPreferenceParameterValue("CHECK_STOCKS_MESSAGE"));
        public static bool DatabaseBackupAutomaticEnabled => Convert.ToBoolean(GetPreferenceParameterValue("DATABASE_BACKUP_AUTOMATIC_ENABLED"));
        public static TimeSpan DatabaseBackupTimeSpan => TimeSpan.Parse(GetPreferenceParameterValue("DATABASE_BACKUP_TIMESPAN"));
        public static TimeSpan DatabaseBackupTimeSpanRangeStart => TimeSpan.Parse(GetPreferenceParameterValue("DATABASE_BACKUP_TIME_SPAN_RANGE_START"));
        public static TimeSpan DatabaseBackupTimeSpanRangeEnd => TimeSpan.Parse(GetPreferenceParameterValue("DATABASE_BACKUP_TIME_SPAN_RANGE_END"));
        public static int NotificationDocumentsToInvoiceIgnoreAfterShowNumberOfTimes => Convert.ToInt16(GetPreferenceParameterValue("NOTIFICATION_DOCUMENTS_TO_INVOICE_IGNORE_AFTER_SHOW_NUMBER_OF_TIMES"));
        public static bool ServiceAtSendDocuments => Convert.ToBoolean(GetPreferenceParameterValue("SERVICE_AT_SEND_DOCUMENTS"));
        public static bool ServiceAtSendDocumentsWaybill => Convert.ToBoolean(GetPreferenceParameterValue("SERVICE_AT_SEND_DOCUMENTS_WAYBILL"));
        public static string SystemCurrency => GetPreferenceParameterValue("SYSTEM_CURRENCY");

        public static CompanyInformations CompanyInformations
        {
            get
            {
                if (_companyInformations == null)
                {
                    LoadCompanyInformations();
                }

                return _companyInformations;
            }
        }

    }
}
