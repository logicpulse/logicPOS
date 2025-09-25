using LogicPOS.Api.Features.System.GetSystemInformations;
using LogicPOS.UI.Errors;

namespace LogicPOS.UI.Services
{
    public static class SystemInformationService
    {
        private static SystemInformation _systemInfo;
       
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
                ErrorHandlingService.HandleApiError(result,true);
                return Default;
            }

            return result.Value;
        }
    }
}
