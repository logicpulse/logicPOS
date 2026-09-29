using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.VatRates.GetAllVatRate 
{ 
    public class GetAllVatRatesQuery : IRequest<ErrorOr<IEnumerable<VatRate>>>
    {
    }
}
