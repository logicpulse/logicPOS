using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.System.Licensing.AddMessage
{
    public class AddMessageCommandHandler : RequestHandler<AddMessageCommand, ErrorOr<AddMessageResponse>>
    {
        public AddMessageCommandHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override Task<ErrorOr<AddMessageResponse>> Handle(AddMessageCommand request, CancellationToken ct = default)
        {
            return HandlePostCommandAsync<AddMessageResponse>("licensing/add-message", request, ct);
        }
    }
}
