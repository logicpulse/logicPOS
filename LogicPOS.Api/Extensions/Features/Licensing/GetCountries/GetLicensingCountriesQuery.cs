using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.Licensing.GetCountries
{
    public class GetLicensingCountriesQuery : IRequest<ErrorOr<IEnumerable<string>>>
    {

    }
}
