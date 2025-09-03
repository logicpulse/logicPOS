using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Company.GetCompanyCurreny;
using LogicPOS.Api.Features.Currencies.GetAllCurrencies;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.Currencies
{
    public static class CurrenciesService
    {
        private static Currency _default;
        private static List<Currency> _currencies;

        public static List<Currency> Currencies
        {
            get
            {
                if (_currencies == null)
                {
                   _currencies = GetAllCurrencies();
                }
                return _currencies;
            }
        }

        private static List<Currency> GetAllCurrencies()
        {
            var currencies = DependencyInjection.Mediator.Send(new GetAllCurrenciesQuery()).Result;

            if (currencies.IsError)
            {
                ErrorHandlingService.HandleApiError(currencies);
                return null;
            }

            return currencies.Value.ToList();
        }

        public static Currency Default
        {
            get
            {
                if (_default == null)
                {
                    _default = GetCompanyCurreny();
                }

                return _default;
            }
        }

        private static Currency GetCompanyCurreny()
        {
            var currency = DependencyInjection.Mediator.Send(new GetCompanyCurrencyQuery()).Result;

            if (currency.IsError)
            {
                ErrorHandlingService.HandleApiError(currency);
                return null;
            }

            return currency.Value;
        }
         
        
    }
}
