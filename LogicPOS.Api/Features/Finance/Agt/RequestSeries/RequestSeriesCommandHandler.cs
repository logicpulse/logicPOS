using ErrorOr;
using LogicPOS.Api.Features.Common.Caching;
using LogicPOS.Api.Features.Common.Requests;
using LogicPOS.Api.Features.Finance.Documents.Documents;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.RequestSeries
{
    public class RequestSeriesCommandHandler : RequestHandler<RequestSeriesCommand, ErrorOr<AgtSeriesInfo>>
    {
        public RequestSeriesCommandHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<AgtSeriesInfo>> Handle(RequestSeriesCommand request, CancellationToken cancellationToken = default)
        {
            var result =  await HandlePostCommandAsync<AgtSeriesInfo>("agt/documents/series", request, cancellationToken);

            return result;
        }
    }
}
