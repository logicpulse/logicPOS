using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.PoleDisplays.AddPoleDisplay
{
    public class AddPoleDisplayCommandHandler :
        RequestHandler<AddPoleDisplayCommand, ErrorOr<Guid>>
    {
        public AddPoleDisplayCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<Guid>> Handle(AddPoleDisplayCommand command, CancellationToken cancellationToken = default)
        {
            return await HandleAddCommandAsync("poledisplays",command, cancellationToken);
        }
    }
}
