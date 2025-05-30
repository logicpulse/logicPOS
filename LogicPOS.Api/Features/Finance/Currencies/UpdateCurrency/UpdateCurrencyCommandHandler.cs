using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Currencies.UpdateCurrency
{
    public class UpdateCurrencyCommandHandler : RequestHandler<UpdateCurrencyCommand, ErrorOr<Unit>>
    {
        public UpdateCurrencyCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdateCurrencyCommand command,
                                                           CancellationToken cancellationToken = default)
        {
           return await HandleUpdateCommandAsync($"currencies/{command.Id}", command, cancellationToken);
        }
    }
}
