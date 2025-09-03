using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Countries.GetAllCountries;
using LogicPOS.UI.Alerts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Services
{
    public static class CountriesService
    {
        private static List<Country> _countries;
        private static Country _default;

        public static List<Country> Countries
        {
            get
            {
                if (_countries == null)
                {
                    _countries = GetAllCountries();
                }
                return _countries;
            }
        }

        public static Country Default
        {
            get
            {
                if (_default == null)
                {
                    _default = Countries.FirstOrDefault(c => c.Code2 == PreferenceParametersService.CompanyInformations.CountryCode2);
                }
                return _default;
            }
        }

        private static List<Country> GetAllCountries()
        {
            var countriesResult = DependencyInjection.Mediator.Send(new GetAllCountriesQuery()).Result;
            if (countriesResult.IsError)
            {
                SimpleAlerts.ShowApiErrorAlert(countriesResult.FirstError);
                return null;
            }
            return countriesResult.Value.ToList();
        }

        public static Country GetCountry(Guid id)
        {
            return Countries.FirstOrDefault(c => c.Id == id);
        }
    }
}
