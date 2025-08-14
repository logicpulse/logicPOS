using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Countries.GetAllCountries;
using LogicPOS.UI.Alerts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogicPOS.UI.Services
{
    public static class CountryService
    {
        private static ISender _mediator = DependencyInjection.Mediator;
        public static Country DefaultCountry=> CountryService.countries.FirstOrDefault(c => c.Code2 == PreferenceParametersService.CompanyInformations.CountryCode2);
        public static List<Country> countries=> GetCountries();
        public static List<Country> GetCountries()
        {
            var countriesResult = _mediator.Send(new GetAllCountriesQuery()).Result;
            if (countriesResult.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(countriesResult.FirstError);
                return null;
            }
            return countriesResult.Value.ToList();
        }
    }
}
