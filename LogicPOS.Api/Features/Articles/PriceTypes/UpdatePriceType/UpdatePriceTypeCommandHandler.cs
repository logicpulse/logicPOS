using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using MediatR;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.PriceTypes.UpdatePriceType
{
    public class UpdatePriceTypeCommandHandler :
        RequestHandler<UpdatePriceTypeCommand, ErrorOr<Unit>>
    {
        public UpdatePriceTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Unit>> Handle(UpdatePriceTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleUpdateCommand($"/articles/pricetypes/{command.Id}", command, cancellationToken);
        }
    }
}
