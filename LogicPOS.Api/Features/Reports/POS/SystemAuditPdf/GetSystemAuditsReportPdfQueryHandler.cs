using ErrorOr;
using LogicPOS.Api.Extensions;
using LogicPOS.Api.Features.Common.Requests;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace LogicPOS.Api.Features.Reports.SystemAudits
{
    public class GetSystemAuditsReportPdfQueryHandler :
        RequestHandler<GetSystemAuditsReportPdfQuery, ErrorOr<TempFile>>
    {
        public GetSystemAuditsReportPdfQueryHandler(IHttpClientFactory factory) : base(factory)
        {
        }

        public async override Task<ErrorOr<TempFile>> Handle(GetSystemAuditsReportPdfQuery query, CancellationToken cancellationToken = default)
        {
            string endpoint = $"reports/{query.TerminalId}/system-audits/pdf?startDate={query.StartDate.ToISO8601DateOnly()}&endDate={query.EndDate.ToISO8601DateOnly()}";
            return await HandleGetFileQueryAsync(endpoint, cancellationToken);
        }
    }
}
