using ErrorOr;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Common;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.PriceTypes.AddPriceType
{
    public class AddPriceTypeCommandHandler : RequestHandler<AddPriceTypeCommand, ErrorOr<Guid>>
    {
        public AddPriceTypeCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPriceTypeCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommand("articles/pricetypes", command, cancellationToken);
        }
    }
}
