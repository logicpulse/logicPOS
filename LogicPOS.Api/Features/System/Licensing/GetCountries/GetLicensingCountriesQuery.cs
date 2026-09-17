using ErrorOr;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.Api.Features.System.Licensing.GetCountries
{
    public class GetLicensingCountriesQuery : IRequest<ErrorOr<IEnumerable<string>>>
    {

    }
}
