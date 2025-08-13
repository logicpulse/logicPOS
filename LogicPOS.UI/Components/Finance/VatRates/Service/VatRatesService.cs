using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.VatRates.GetAllVatRate;
using LogicPOS.UI.Errors;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Finance.VatRates.Service
{
    public static class VatRatesService
    {
        private static readonly ISender _mediator = DependencyInjection.Mediator;

        public static List<VatRate> GetAllVatRates()
        {
            var result = _mediator.Send(new GetAllVatRatesQuery()).Result;
            if (result.IsError) 
            { 
                ErrorHandlingService.HandleApiError (result);
                return null;
            }
            var vatRates= result.Value.ToList();
            return vatRates;
        }
    }
}
