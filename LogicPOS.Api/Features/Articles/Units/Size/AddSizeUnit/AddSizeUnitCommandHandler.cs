using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.SizeUnits.AddSizeUnit
{
    public class AddSizeUnitCommandHandler : RequestHandler<AddSizeUnitCommand, ErrorOr<Guid>>
    {
        public AddSizeUnitCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddSizeUnitCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("articles/sizeunits", command, cancellationToken);
        }
    }
}
