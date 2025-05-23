using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Currencies.GetAllCurrencies
{
    public class GetAllCurrenciesQuery : IRequest<ErrorOr<IEnumerable<Currency>>>
    {
    }
}
