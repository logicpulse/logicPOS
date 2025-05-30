using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Company.GetCompanyCurreny
{
    public class GetCompanyCurrencyQueryHandler :
        RequestHandler<GetCompanyCurrencyQuery, ErrorOr<Currency>>
    {
        public GetCompanyCurrencyQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<Currency>> Handle(GetCompanyCurrencyQuery request, CancellationToken cancellationToken = default)
        {
            return await HandleGetEntityQueryAsync<Currency>("company/currency", cancellationToken);
        }
    }
}
