using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Places.AddPlace
{
    public class AddPlaceCommandHandler
        : RequestHandler<AddPlaceCommand, ErrorOr<Guid>>
    {
        public AddPlaceCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPlaceCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("places", command, cancellationToken);
        }
    }
}
