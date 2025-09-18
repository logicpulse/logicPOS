using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common.Requests;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.PriceTypes.UpdatePriceType
{
    public class UpdatePriceTypeCommandHandler :
        RequestHandler<UpdatePriceTypeCommand, ErrorOr<Success>>
    {
        public UpdatePriceTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(UpdatePriceTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommandAsync($"/articles/pricetypes/{command.Id}", command, cancellationToken);
        }
    }
}
