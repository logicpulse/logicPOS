using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Documents.Series.CreateDefaultSeries
{
    public class CreateDefaultSeriesCommandHandler : RequestHandler<CreateDefaultSeriesCommand, ErrorOr<Success>>
    {
        public CreateDefaultSeriesCommandHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<Success>> Handle(CreateDefaultSeriesCommand request, CancellationToken cancellationToken = default)
        {
            var result = await HandlePostCommandAsync<object>("documents/series/default", request, cancellationToken);
            if (result.IsError)
            {
                return result.Errors;
            }

            return Result.Success;
        }
    }
}
