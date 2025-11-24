using ErrorOr;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.FiscalYears.GetCurrentFiscalYear
{
    public class GetCurrentFiscalYearQueryHandler : RequestHandler<GetCurrentFiscalYearQuery, ErrorOr<FiscalYear>>
    {
        public GetCurrentFiscalYearQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<FiscalYear>> Handle(GetCurrentFiscalYearQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<FiscalYear>($"fiscalyears/current", cancellationToken);
        }
    }
}
