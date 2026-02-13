using LogicPOS.Api.Features.System.GetApiVersion;
using LogicPOS.Api.Features.System.GetSystemInformations;
using LogicPOS.UI.Components.Licensing;
using LogicPOS.UI.Errors;
using Serilog;
using System;

namespace LogicPOS.UI.Services
{
    public static class SystemInformationService
    {
        private static SystemInformation _systemInfo;
        private static Version _apiVersion;

        public static SystemInformation SystemInformation
        {
            get
            {
                if (_systemInfo == null)
                {
                    _systemInfo = GetSystemInformation();
                }

                return _systemInfo;
            }
        }

        private static SystemInformation Default => new SystemInformation
        {
            Culture = "pt-PT",
            CountryCode2 = "PT"
        };

        private static SystemInformation GetSystemInformation()
        {
            var result = DependencyInjection.Mediator.Send(new GetSystemInformationsQuery()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, true);
                return Default;
            }

            return result.Value;
        }

        public static Version ApiVersion
        {
            get
            {
                if (_apiVersion == null) {
                    _apiVersion = GetApiVersion();
                }

                return _apiVersion;
            }
        }

        private static Version GetApiVersion()
        {
            var result = DependencyInjection.Mediator.Send(new GetApiVersionQuery()).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, true);
                return new Version(0, 0, 0);
            }

            if (Version.TryParse(result.Value, out var latestVersion))
            {
                return latestVersion;
            }
            else
            {
                Log.Warning("Received invalid version format from API: " + result.Value);
                return new Version(0, 0, 0);
            }
        }

        public static bool UseAgtFe => SystemInformation.IsAngola && LicensingService.Data.AgtFeModule;
    }
}
