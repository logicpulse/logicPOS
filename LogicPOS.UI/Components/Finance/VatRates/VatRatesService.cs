using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.VatRates.GetAllVatRate;
using LogicPOS.UI.Errors;
using System;
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

        public static VatRate GetById(Guid id) => VatRates.Where(x =>  x.Id == id).FirstOrDefault();

        private static List<VatRate> GetAll()
        {
            var vatRates = DependencyInjection.Mediator.Send(new GetAllVatRatesQuery()).Result;

            if (vatRates.IsError)
            {
                ErrorHandlingService.HandleApiError(vatRates);
                return null;
            }

            return vatRates.Value.ToList();
        }
    }
}
