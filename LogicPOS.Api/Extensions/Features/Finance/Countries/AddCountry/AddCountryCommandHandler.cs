using ErrorOr;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Countries.AddCountry
{
    public class AddCountryCommandHandler : RequestHandler<AddCountryCommand, ErrorOr<Guid>>
    {
        public AddCountryCommandHandler(IHttpClientFactory httpClientFactory) : base(httpClientFactory) { }

        public override async Task<ErrorOr<Guid>> Handle(
            AddCountryCommand command,
            CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("countries", command, cancellationToken);
        }
    }
}
