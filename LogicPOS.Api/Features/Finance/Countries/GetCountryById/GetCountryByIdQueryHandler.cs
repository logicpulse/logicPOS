using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Countries.GetCountryById
{
    public class GetCountryByIdQueryHandler : RequestHandler<GetCountryByIdQuery, ErrorOr<Country>>
    {
        public GetCountryByIdQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Country>> Handle(GetCountryByIdQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<Country>($"countries/{query.Id}", cancellationToken);
        }
    }
}
