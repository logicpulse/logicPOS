using LogicPOS.Api.Features.Licensing.GetCountries;
using LogicPOS.UI.Errors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Licensing
{
    public static class LicensingService
    {
        private static readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<IMediator>();

        public static IEnumerable<string> GetCountries()
        {
            var result = _mediator.Send(new GetLicensingCountriesQuery()).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result);
                return Enumerable.Empty<string>();
            }

            return result.Value;
        }
    }
}
