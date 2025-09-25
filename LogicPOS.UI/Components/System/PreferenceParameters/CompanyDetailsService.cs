using LogicPOS.Api.Features.Company;
using LogicPOS.Api.Features.Company.GetCompanyInformations;
using LogicPOS.Api.Features.Company.UpdateCompanyDetails;
using LogicPOS.UI.Errors;
using Serilog;
using System;

namespace LogicPOS.UI.Services
{
    public static class CompanyDetailsService
    {
        private static CompanyInformation _companyInformations;


        public static CompanyInformation CompanyInformation
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

        private static void LoadCompanyInformations()
        {
            var companyInformations = DependencyInjection.Mediator.Send(new GetCompanyInformationsQuery()).Result;

            if (companyInformations.IsError)
            {
                ErrorHandlingService.HandleApiError(companyInformations, false);
                _companyInformations = new CompanyInformation
                {
                    CountryCode2 = "PT"
                };
                return;
            }

            _companyInformations = companyInformations.Value;
        }

        public static void UpdateCompanyDetails(UpdateCompanyDetailsCommand command)
        {

            var result = DependencyInjection.Mediator.Send(command).Result;
           
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, false);
                return;
            }

            LoadCompanyInformations();
        }

        public static bool CompnayIsConfigured()
        {
            return string.IsNullOrWhiteSpace(CompanyInformation.FiscalNumber) == false &&
                   string.IsNullOrWhiteSpace(_companyInformations.Name) == false;
        }
    }
}
