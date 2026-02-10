using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Countries.GetAllCountries;
using LogicPOS.UI.Errors;
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
                    _default = Countries.FirstOrDefault(c => c.Code2.ToLower() == SystemInformationService.SystemInformation.CountryCode2.ToLower());
                }
                return _default;
            }
        }

        private static List<Country> GetAllCountries()
        {
            var countriesResult = DependencyInjection.Mediator.Send(new GetAllCountriesQuery()).Result;
            if (countriesResult.IsError)
            {
                ErrorHandlingService.HandleApiError(countriesResult);
                return null;
            }
            return countriesResult.Value.ToList();
        }

        public static List<AutoCompleteLine> AutocompleteLines => Countries.Select(c => new AutoCompleteLine
        {
            Id = c.Id,
            Name = c.Designation
        }).ToList();

        public static Country GetById(Guid id)
        {
            return Countries.FirstOrDefault(c => c.Id == id);
        }

        public static Country GetByCode2(string code2)
        {
            return Countries.FirstOrDefault(c => c.Code2.ToLower() == code2.ToLower());
        }
    }
}
