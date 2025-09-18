using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Countries.UpdateCountry
{
    public class UpdateCountryCommandHandler :
        RequestHandler<UpdateCountryCommand, ErrorOr<Success>>
    {
        public UpdateCountryCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdateCountryCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"countries/{command.Id}", command, cancellationToken);
        }
    }
}
