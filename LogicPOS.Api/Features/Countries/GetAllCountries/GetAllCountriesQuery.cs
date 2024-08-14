using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Countries.GetAllCountries
{
    public class GetAllCountriesQuery : IRequest<ErrorOr<IEnumerable<Country>>>
    {

    }
}
