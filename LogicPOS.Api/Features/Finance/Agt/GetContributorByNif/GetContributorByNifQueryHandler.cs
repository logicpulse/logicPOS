using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.Agt.GetContributorByNif
{
    public class GetContributorByNifQueryHandler : RequestHandler<GetContributorByNifQuery, ErrorOr<Contributor>>
    {
        public GetContributorByNifQueryHandler(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public override async Task<ErrorOr<Contributor>> Handle(GetContributorByNifQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<Contributor>($"agt/contributors/by-nif?Nif={request.Nif}",cancellationToken);
        }
    }
}
