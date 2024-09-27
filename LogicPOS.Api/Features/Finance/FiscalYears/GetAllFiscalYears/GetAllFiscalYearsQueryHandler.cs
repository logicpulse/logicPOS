using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.FiscalYears.GetAllFiscalYears
{
    public class GetAllFiscalYearsQueryHandler :
        RequestHandler<GetAllFiscalYearsQuery, ErrorOr<IEnumerable<FiscalYear>>>
    {
        public GetAllFiscalYearsQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public override async Task<ErrorOr<IEnumerable<FiscalYear>>> Handle(GetAllFiscalYearsQuery query,
                                                                     CancellationToken cancellationToken = default)
        {
            return await HandleGetAllQueryAsync<FiscalYear>("fiscalyears", cancellationToken);
        }
    }
}
