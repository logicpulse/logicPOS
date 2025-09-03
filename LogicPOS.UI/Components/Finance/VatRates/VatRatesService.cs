using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.VatRates.GetAllVatRate;
using LogicPOS.UI.Errors;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Finance.VatRates.Service
{
    public static class VatRatesService
    {
        private static List<VatRate> _vats;

        public static List<VatRate> VatRates
        {
            get
            {
                if (_vats == null)
                {
                    _vats = GetAll();
                }
                return _vats;
            }
        }

        private static List<VatRate> GetAll()
        {
            var getReasons = DependencyInjection.Mediator.Send(new GetAllVatRatesQuery()).Result;

            if (getReasons.IsError)
            {
                ErrorHandlingService.HandleApiError(getReasons);
                return null;
            }

            return getReasons.Value.ToList();
        }
    }
}
