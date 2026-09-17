using ErrorOr;
using LogicPOS.Api.Entities;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Countries.GetAllCountries
{
    public class GetAllCountriesQuery : IRequest<ErrorOr<IEnumerable<Country>>>
    {

    }
}
