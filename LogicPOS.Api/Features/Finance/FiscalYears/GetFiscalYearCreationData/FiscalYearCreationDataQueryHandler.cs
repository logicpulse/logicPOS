using ErrorOr;
using LogicPOS.Api.Features.Common.Requests;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Finance.FiscalYears.GetFiscalYearCreationData
{
    public class FiscalYearCreationDataQueryHandler : RequestHandler<GetFiscalYearCreationDataQuery, ErrorOr<FiscalYearCreationData>>
    {
        public FiscalYearCreationDataQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<FiscalYearCreationData>> Handle(GetFiscalYearCreationDataQuery query, CancellationToken cancellationToken = default)
        {
            return await HandleGetQueryAsync<FiscalYearCreationData>($"fiscal-years/creation-data", cancellationToken);
        }
    }
}
