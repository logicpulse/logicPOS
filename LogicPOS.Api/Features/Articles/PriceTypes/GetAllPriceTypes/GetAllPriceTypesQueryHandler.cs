using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Articles.PriceTypes.GetAllPriceTypes
{
    public class GetAllPriceTypesQueryHandler :
        RequestHandler<GetAllPriceTypesQuery, ErrorOr<IEnumerable<PriceType>>>
    {
        public GetAllPriceTypesQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<PriceType>>> Handle(GetAllPriceTypesQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<PriceType>("articles/pricetypes", cancellationToken);
        }
    }
}
