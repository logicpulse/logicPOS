using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PreferenceParameters.GetAllPreferenceParameters;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Services
{
    public static class PreferenceParametersService
    {
        public static List<PreferenceParameter> _preferenceParameters;

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
                ErrorHandlingService.HandleApiError(preferenceParameters);
                return;
            }

            _preferenceParameters = preferenceParameters.Value.ToList();
        }
        
        public static void RefreshPreferenceParameters()
        {
            LoadPreferenceParameters();
        }

        public static string SaftExportPath => GetPreferenceParameterValue("PATH_SAFTPT");
        public static bool CheckStocks => Convert.ToBoolean(GetPreferenceParameterValue("CHECK_STOCKS"));
        public static bool CheckStocksMessage => Convert.ToBoolean(GetPreferenceParameterValue("CHECK_STOCKS_MESSAGE"));
        public static bool PrintTicket => Convert.ToBoolean(GetPreferenceParameterValue("TICKET_PRINT_TICKET")); 
        public static bool PrintComercialName => Convert.ToBoolean(GetPreferenceParameterValue("TICKET_PRINT_COMERCIAL_NAME"));
        public static string SystemCurrency => GetPreferenceParameterValue("SYSTEM_CURRENCY");
        public static string AgtLogo => GetPreferenceParameterValue("AGT_FE_QRCODE_LOGO");
        public static int MaxAccountSplitterNumber => int.Parse(GetPreferenceParameterValue("SPLIT_PAYMENT_MAX_CLIENTS"));
    }
}
